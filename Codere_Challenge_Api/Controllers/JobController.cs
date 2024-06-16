using Codere_Challenge_Jobs.Jobs;
using Codere_Challenge_Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codere_Challenge_Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;
        private readonly IFetchShowsJob _fetchShowsJob;
        private readonly IJobStatusService _jobStatusService;
        private readonly IJobExecutionService _jobExecutionService;

        public JobController(ILogger<JobController> logger, IFetchShowsJob fetchShowsJob, IJobStatusService jobStatusService, IJobExecutionService jobExecutionService)
        {
            _logger = logger;
            _fetchShowsJob = fetchShowsJob;
            _jobStatusService = jobStatusService;
            _jobExecutionService = jobExecutionService;
        }

        [HttpPost("run-fetch-shows")]
        public IActionResult RunFetchShowsJob()
        {
            if (_jobStatusService.IsJobRunning)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    "An instance of the job is already running, Please try again later");
            }

            BackgroundJob.Enqueue(() => _fetchShowsJob.ExecuteAsync());
            return Ok("Job has been scheduled to fetch and store shows.");
        }

        [HttpGet("GetAllExecutions")]
        public IActionResult GetAllExecutions()
        {
            return Ok(_jobExecutionService.GetAllAsync());
        }
    }
}
