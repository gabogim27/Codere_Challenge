using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Settings;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Jobs.Jobs;
using Codere_Challenge_Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Text.Json;

namespace Codere_Challenge_Jobs_Unit_Tests.Jobs
{
    [TestFixture]
    public class FetchShowsJobTests
    {
        private Mock<IShowService> _mockShowService;
        private Mock<IJobStatusService> _mockJobStatusService;
        private Mock<IJobExecutionService> _mockJobExecutionService;
        private Mock<ILogger<FetchShowsJob>> _mockLogger;
        private HttpClient _httpClient;
        private JobSettings _jobSettings;
        private FetchShowsJob _fetchShowsJob;
        private MockHttpMessageHandler _httpMessageHandler;

        [SetUp]
        public void SetUp()
        {
            _mockShowService = new Mock<IShowService>();
            _mockJobStatusService = new Mock<IJobStatusService>();
            _mockJobExecutionService = new Mock<IJobExecutionService>();
            _mockLogger = new Mock<ILogger<FetchShowsJob>>();

            _jobSettings = new JobSettings { BatchSize = 50, TvMazeApiUrl = "https://api.tvmaze.com/shows" };
            _httpMessageHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_httpMessageHandler);

            var options = Options.Create(_jobSettings);

            _fetchShowsJob = new FetchShowsJob(
                _mockLogger.Object,
                _mockShowService.Object,
                _httpClient,
                options,
                _mockJobStatusService.Object,
                _mockJobExecutionService.Object);
        }

        [TearDown]
        public void Dispose()
        {
            _httpMessageHandler.Dispose();
            _httpClient.Dispose();
        }

        [Test]
        public async Task ExecuteAsyncJobAlreadyRunningTest()
        {
            _mockJobStatusService.Setup(s => s.IsJobRunning).Returns(true);
            
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _fetchShowsJob.ExecuteAsync());
            Assert.That(ex.Message, Is.EqualTo("Job is already running."));
        }

        [Test]
        public async Task ExecuteAsyncFetchShowsSuccessTest()
        {
            var showList = new List<Show>
            {
                new Show { Id = 1, Name = "Show1" },
                new Show { Id = 2, Name = "Show2" }
            };

            _mockJobStatusService.Setup(s => s.IsJobRunning).Returns(false);
            _mockJobExecutionService.Setup(s => s.GetLatestJob()).ReturnsAsync((JobExecutionStatus)null);

            _httpMessageHandler.SetHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(showList))
            });

            _mockShowService.Setup(s => s.GetShowByListOfIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Show>());

            await _fetchShowsJob.ExecuteAsync();

            _mockShowService.Verify(s => s.AddShowAsync(It.IsAny<Show>()), Times.Exactly(showList.Count));
            _mockJobExecutionService.Verify(s => s.AddOrUpdateJobStatusAsync(It.IsAny<JobExecutionStatus>()), Times.AtLeastOnce);
            _mockJobStatusService.Verify(s => s.SetJobRunning(false), Times.Once);
        }

        [Test]
        public async Task ExecuteAsyncFetchShowsHttpRequestExceptionTest()
        {
            _mockJobStatusService.Setup(s => s.IsJobRunning).Returns(false);
            _mockJobExecutionService.Setup(s => s.GetLatestJob()).ReturnsAsync((JobExecutionStatus)null);

            _httpMessageHandler.SetHttpResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

            await _fetchShowsJob.ExecuteAsync();

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<HttpRequestException>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
            _mockJobStatusService.Verify(s => s.SetJobRunning(false), Times.Once);
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage _httpResponseMessage;

        public void SetHttpResponse(HttpResponseMessage httpResponseMessage)
        {
            _httpResponseMessage = httpResponseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return await Task.FromResult(_httpResponseMessage);
        }
    }
}
