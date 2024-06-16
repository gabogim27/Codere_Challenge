using Codere_Challenge_Jobs.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Codere_Challenge_Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
   // [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IFetchShowsJob _fetchShowsJob;
        private readonly IJobStatusService _jobStatusService;

        public JobController(ILogger<JobController> logger, IConfiguration configuration, IFetchShowsJob fetchShowsJob, IJobStatusService jobStatusService)
        {
            _logger = logger;
            _configuration = configuration;
            _fetchShowsJob = fetchShowsJob;
            _jobStatusService = jobStatusService;
        }

        [HttpPost("run-fetch-shows")]
        public IActionResult RunFetchShowsJob([FromHeader(Name = "x-api-key")] string apiKey)
        {
            //if (apiKey != _configuration["ApiKey"])
            //{
            //    return Unauthorized();
            //}

            if (_jobStatusService.IsJobRunning)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    "An instance of the job is already running, Please try again later");
            }

            BackgroundJob.Enqueue(() => _fetchShowsJob.ExecuteAsync());
            return Ok("Job has been scheduled to fetch and store shows.");
        }
    }
}
