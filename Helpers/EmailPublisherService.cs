namespace GCP.Demo;

public class EmailPublisherService : PublisherService
{
    private ILogger<EmailPublisherService> _log;

    public EmailPublisherService(IConfiguration config, ILogger<EmailPublisherService> log) : base(config, log, "Email")
    {
        _log = log;
    }
}