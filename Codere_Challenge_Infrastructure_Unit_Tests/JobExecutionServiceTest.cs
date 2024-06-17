using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Services.Implementations;
using Moq;

namespace Codere_Challenge_Services_Unit_Tests
{
    [TestFixture]
    public class JobExecutionServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IJobExecutionRepository> _mockJobExecutionRepository;
        private JobExecutionService _jobExecutionService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockJobExecutionRepository = new Mock<IJobExecutionRepository>();
            _mockUnitOfWork.Setup(u => u.JobExecution).Returns(_mockJobExecutionRepository.Object);

            _jobExecutionService = new JobExecutionService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task GetJobStatusByIdAsyncValidIdTest()
        {
            var jobStatus = new JobExecutionStatus { Id = 1, JobStatus = JobStatus.FINISHED };
            _mockJobExecutionRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobStatus);
            
            var result = await _jobExecutionService.GetJobStatusByIdAsync(1);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(JobStatus.FINISHED, result.JobStatus);
        }

        [Test]
        public async Task GetAllAsyncTest()
        {
            var jobStatuses = new List<JobExecutionStatus>
            {
                new JobExecutionStatus { Id = 1, JobStatus = JobStatus.FINISHED },
                new JobExecutionStatus { Id = 0, JobStatus = JobStatus.INPROCESS }
            };
            _mockJobExecutionRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(jobStatuses);

            var result = await _jobExecutionService.GetAllAsync();

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task AddOrUpdateJobStatusAsyncTest()
        {
            var jobStatus = new JobExecutionStatus { Id = 0, JobStatus = JobStatus.INPROCESS };
            _mockJobExecutionRepository.Setup(r => r.GetByIdAsync(jobStatus.Id)).ReturnsAsync(jobStatus);

            await _jobExecutionService.AddOrUpdateJobStatusAsync(jobStatus);

            _mockJobExecutionRepository.Verify(r => r.UpdateAsync(jobStatus), Times.Once);
            _mockJobExecutionRepository.Verify(r => r.AddAsync(It.IsAny<JobExecutionStatus>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task AddOrUpdateJobStatusAsyncNewJobStatusTest()
        {
            var jobStatus = new JobExecutionStatus { Id = 0, JobStatus = JobStatus.INPROCESS };
            _mockJobExecutionRepository.Setup(r => r.GetByIdAsync(jobStatus.Id)).ReturnsAsync((JobExecutionStatus)null);

            await _jobExecutionService.AddOrUpdateJobStatusAsync(jobStatus);

            _mockJobExecutionRepository.Verify(r => r.UpdateAsync(It.IsAny<JobExecutionStatus>()), Times.Never);
            _mockJobExecutionRepository.Verify(r => r.AddAsync(jobStatus), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task GetLatestJobTest()
        {
            var jobStatus = new JobExecutionStatus { Id = 1, JobStatus = JobStatus.FINISHED };
            _mockJobExecutionRepository.Setup(r => r.GetLatestJob()).ReturnsAsync(jobStatus);

            var result = await _jobExecutionService.GetLatestJob();

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(JobStatus.FINISHED, result.JobStatus);
        }
    }
}
