using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GCP.Demo.Helpers;

public class Event
{
    public Guid Id { get; set; }

    public string Type { get; set; }

    public string Date { get; set; }

    public string Message { get; set; }
}