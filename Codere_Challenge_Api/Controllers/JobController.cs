using Codere_Challenge_Jobs.Jobs;
using Codere_Challenge_Services.Implementations;
using Codere_Challenge_Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codere_Challenge_Api.Controllers
{
    /// <summary>
    /// Controller for managing background jobs related to fetching shows.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "ApiKey")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;
        private readonly IFetchShowsJob _fetchShowsJob;
        private readonly IJobStatusService _jobStatusService;
        private readonly IJobExecutionService _jobExecutionService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public JobController(ILogger<JobController> logger, IFetchShowsJob fetchShowsJob, IJobStatusService jobStatusService, IJobExecutionService jobExecutionService, IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _fetchShowsJob = fetchShowsJob;
            _jobStatusService = jobStatusService;
            _jobExecutionService = jobExecutionService;
            _backgroundJobClient = backgroundJobClient;
        }

        /// <summary>
        /// Schedules a job to fetch and store shows if no other instance of the job is running.
        /// </summary>
        /// <returns>Status indicating if the job was scheduled or if a conflict occurred.</returns>
        [HttpPost("run-fetch-shows")]
        public IActionResult RunFetchShowsJob()
        {
            if (_jobStatusService.IsJobRunning)
            {
                return StatusCode(StatusCodes.Status409Conflict,
                    "An instance of the job is already running, Please try again later");
            }

            _backgroundJobClient.Enqueue(() => _fetchShowsJob.ExecuteAsync());
            return Ok("Job has been scheduled to fetch and store shows.");
        }

        /// <summary>
        /// Retrieves all job execution statuses.
        /// </summary>
        /// <returns>A list of all job execution statuses.</returns>
        [HttpGet("GetAllExecutions")]
        public async Task<ActionResult<IEnumerable<JobExecutionService>>> GetAllExecutions()
        {
            var jobs = await _jobExecutionService.GetAllAsync();
            return Ok(jobs);
        }
    }
}
