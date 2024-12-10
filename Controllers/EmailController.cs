using GCP.Demo.Helpers;
using GCP.Demo.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;



namespace GCP.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly EmailPublisherService _emailPublisher;

        public EmailController(ILogger<EmailController> logger, EmailPublisherService emailPublisher)
        {
            _logger = logger;
            _emailPublisher = emailPublisher;
        }

        [HttpPost(ApiEndpoints.Email.Send)]
        public async Task<IActionResult> Send([FromBody] SendEmailRequest request, CancellationToken token)
        {
            var evntobj = new Event
            {
                Id = Guid.NewGuid(),
                Type = "Email",
                Date = DateTime.UtcNow.ToString(),
                Message = JsonSerializer.Serialize<SendEmailRequest>(request)
            };
            try
            {
                await _emailPublisher.PublishMessageWithRetrySettingsAsync(evntobj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(ex.Message);
            }



            return Ok(evntobj.Id);
        }
    }
}
