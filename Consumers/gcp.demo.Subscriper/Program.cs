using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using static Google.Rpc.Context.AttributeContext.Types;
namespace gcp.demo.Subscriper
{
    internal class Program
    {
        private const string projectId = "";
        private const string subscriptionId = "";
        private static SubscriptionName _subscriptionName;
        private static SubscriberClient _subscriber;
        private static ILogger _logger;

        static void Main(string[] args)
        {
            //var services = new ServiceCollection() as IServiceCollection;
            //var logger = services.GetRequiredService<ILogger<DefaultGoogleCloudPubSubPersistentConnection>>();
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            _logger = loggerFactory.CreateLogger<Program>();
            _logger.LogInformation("Example log message");
            var binDirectory = Directory.GetCurrentDirectory();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $"{binDirectory}//training-415008-12146157e296.json");
            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            _logger.LogInformation($"projectId : {projectId}");
            _logger.LogInformation($"subscriptionId : {subscriptionId}");
            _logger.LogInformation($"subscriptionName : {subscriptionName}");
            _subscriber = new SubscriberClientBuilder
            {
                Settings = new SubscriberClient.Settings
                {

                },
                GrpcAdapter = GrpcNetClientAdapter.Default,
                SubscriptionName = subscriptionName

            }.Build();

            Run().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async System.Threading.Tasks.Task Run()
        {


            await _subscriber.StartAsync(async (message, cancellationToken) =>
            {
                try
                {
                    var messageData = message.Data.ToStringUtf8();
                    Event data = JsonSerializer.Deserialize<Event>(messageData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


                    _logger.LogInformation($"Event id : {data.Id} Type: {data.Type}");
                    EmailRequest email = JsonSerializer.Deserialize<EmailRequest>(data.Message);

                    var client = new SmtpClient("mail5013.site4now.net", 587)
                    {
                        Credentials = new NetworkCredential("", ""),
                        EnableSsl = false
                    };
                    client.Send("itservice@saudilightech.com", email.To[0], email.Subject, email.Body);
                    _logger.LogInformation($"Email {data.Id} Was Sent ");
                    return SubscriberClient.Reply.Ack;
                }
                catch (Exception ex)
                {
                    
                    _logger.LogError($"{ex.Message}");

                    throw ex;
                }



            });

            while (true)
            {
                Thread.Sleep(200);
            }
        }


    }


}