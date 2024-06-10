using Codere_Challenge_Jobs.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Codere_Challenge_Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;

        private readonly IConfiguration _configuration;
        private readonly IFetchShowsJob _fetchShowsJob;

        public JobController(ILogger<JobController> logger, IConfiguration configuration, IFetchShowsJob fetchShowsJob)
        {
            _logger = logger;
            _configuration = configuration;
            _fetchShowsJob = fetchShowsJob;
        }

        [HttpPost("run-fetch-shows")]
        public IActionResult RunFetchShowsJob([FromHeader(Name = "x-api-key")] string apiKey)
        {
            if (apiKey != _configuration["ApiKey"])
            {
                return Unauthorized();
            }

            BackgroundJob.Enqueue(() => _fetchShowsJob.ExecuteAsync());
            return Ok("Job has been scheduled to fetch and store shows.");
        }
    }
}
