
public class EmailRequest
{
    public required string Subject { get; init; }
    public required string Body { get; init; }
    public required string[] To { get; init; }
}