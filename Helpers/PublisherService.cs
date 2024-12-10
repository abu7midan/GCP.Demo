using GCP.Demo.Helpers;
using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;
using System.Text.Json;

namespace GCP.Demo;

public class PublisherService
{
    private ILogger<PublisherService> _log;
    private PublisherClient _publisher;
    private TopicName _topicName;
    private int maxAttempts = 3;
    private TimeSpan initialBackoff = TimeSpan.FromMilliseconds(110); // default: 100 ms
    private TimeSpan maxBackoff = TimeSpan.FromSeconds(70); // default : 60 seconds
    private double backoffMultiplier = 1.3; // default: 1.0
    private TimeSpan totalTimeout = TimeSpan.FromSeconds(100); // default: 600 seconds
    public PublisherService(IConfiguration config, ILogger<PublisherService> log, string topicId)
    {
        _log = log;
        //string topicId = config["TopicId"];
        string projectId = config["ProjectId"];
        if (projectId == null)
            throw new ArgumentNullException(
                "You must configure values for PubSub `TopicId`, `ProjectId`");
        try
        {
            if (topicId != null)
            CreateTopicIfNotExist(projectId, topicId);
        }
        catch
        {

        }


        _publisher = new PublisherClientBuilder
        {
            TopicName = _topicName,
            ApiSettings = new PublisherServiceApiSettings
            {
                PublishSettings = CallSettings.FromRetry(RetrySettings.FromExponentialBackoff(
                       maxAttempts: maxAttempts,
                       initialBackoff: initialBackoff,
                       maxBackoff: maxBackoff,
                       backoffMultiplier: backoffMultiplier,
                       retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Unavailable)))
               .WithTimeout(totalTimeout)
            }
        }.Build();
    }
    public Topic CreateTopicIfNotExist(string projectId, string topicId)
    {
        _topicName = TopicName.FromProjectTopic(projectId, topicId);

        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
        Topic topic = null;

        try
        {

            topic = publisher.CreateTopic(_topicName);
            _log.LogInformation($"Topic {topic.Name} created.");
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            _log.LogError($"Topic {topicId} Already Exists.");

        }
        catch (RpcException e)
        {
            throw e;
        }

        return topic;
    }

    public async Task<string> PublishMessageWithRetrySettingsAsync(Event item)
    {

        string data = JsonSerializer.Serialize(item,new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        try
        {
            PubsubMessage message = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(data)
            };
            var response = await _publisher.PublishAsync(message);


            _log.LogInformation($"Published message: {response}");
            return response;
        }
        catch (Exception exception)
        {
            _log.LogError($"An error ocurred when publishing message {data}: " +
                $"{exception.Message}");

            throw exception;
        }
    }
}