namespace GCP.Demo.Requests;

public class SendEmailRequest
{
    public required string Subject { get; init; }
    public required string Body { get; init; }
    public required string[] To { get; init; }
}