using Codere_Challenge_Api.Controllers;
using Codere_Challenge_Core.Entities;
using Codere_Challenge_Jobs.Jobs;
using Codere_Challenge_Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Codere_Challenge_Api_Unit_Tests.Controllers
{
    [TestFixture]
    public class JobControllerTests
    {
        private Mock<ILogger<JobController>> _mockLogger;
        private Mock<IFetchShowsJob> _mockFetchShowsJob;
        private Mock<IJobStatusService> _mockJobStatusService;
        private Mock<IJobExecutionService> _mockJobExecutionService;
        private Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private JobController _jobController;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<JobController>>();
            _mockFetchShowsJob = new Mock<IFetchShowsJob>();
            _mockJobStatusService = new Mock<IJobStatusService>();
            _mockJobExecutionService = new Mock<IJobExecutionService>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            _jobController = new JobController(_mockLogger.Object, _mockFetchShowsJob.Object, _mockJobStatusService.Object, _mockJobExecutionService.Object, _mockBackgroundJobClient.Object);
        }

        [Test]
        public void RunFetchShowsJobReturnsConflictTest()
        {
            _mockJobStatusService.Setup(s => s.IsJobRunning).Returns(true);
            var result = _jobController.RunFetchShowsJob() as ObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
            Assert.That(result.Value, Is.EqualTo("An instance of the job is already running, Please try again later"));
        }

        [Test]
        public void RunFetchShowsJobEnqueuesJobTest()
        {
            _mockJobStatusService.Setup(s => s.IsJobRunning).Returns(false);
            var result = _jobController.RunFetchShowsJob() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.That(result.Value, Is.EqualTo("Job has been scheduled to fetch and store shows."));
        }

        [Test]
        public async Task GetAllExecutionsTest()
        {
            var jobExecutions = new List<JobExecutionStatus>
            {
                new JobExecutionStatus { Id = 1, JobStatus = JobStatus.FINISHED },
                new JobExecutionStatus { Id = 2, JobStatus = JobStatus.INPROCESS }
            };
            _mockJobExecutionService.Setup(s => s.GetAllAsync()).ReturnsAsync(jobExecutions);
            var result = await _jobController.GetAllExecutions();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(jobExecutions));
        }
    }
}
