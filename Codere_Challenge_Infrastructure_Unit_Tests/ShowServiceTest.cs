using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Filters;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Services.Implementations;
using Moq;

namespace Codere_Challenge_Services_Unit_Tests
{
    [TestFixture]
    public class ShowServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IShowRepository> _mockShowRepository;
        private ShowService _showService;

        [SetUp]
        public void SetUp()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockShowRepository = new Mock<IShowRepository>();
            _mockUnitOfWork.Setup(u => u.Shows).Returns(_mockShowRepository.Object);

            _showService = new ShowService(_mockUnitOfWork.Object);
        }

        [Test]
        public async Task GetShowsAsyncTest()
        {
            var shows = new List<Show> { new Show { Id = 1, Name = "Show1" }, new Show { Id = 2, Name = "Show2" } };
            _mockShowRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(shows);

            var result = await _showService.GetShowsAsync();

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetShowByIdAsyncTest()
        {
            var show = new Show { Id = 1, Name = "Show1" };
            _mockShowRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(show);
            
            var result = await _showService.GetShowByIdAsync(1);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Show1", result.Name);
        }

        [Test]
        public async Task GetShowByListOfIdsAsyncTest()
        {
            var ids = new List<int> { 1, 2 };
            var shows = new List<Show> { new Show { Id = 1, Name = "Show1" }, new Show { Id = 2, Name = "Show2" } };
            _mockShowRepository.Setup(r => r.GetByListOfIdsAsync(ids)).ReturnsAsync(shows);

            var result = await _showService.GetShowByListOfIdsAsync(ids);

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task AddShowAsyncTest()
        {
            var show = new Show { Id = 1, Name = "New Show", Network = new Network { Id = 1, Name = "Network" } };

            await _showService.AddShowAsync(show);

            _mockShowRepository.Verify(r => r.AddAsync(show), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateShowsAsyncTest()
        {
            var shows = new List<Show> { new Show { Id = 1, Name = "Show1" }, new Show { Id = 2, Name = "Show2" } };

            await _showService.UpdateShowsAsync(shows);

            _mockShowRepository.Verify(r => r.UpdateRangeAsync(shows), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task GetFilteredShowsAsyncTest()
        {
            var filters = new ShowFilterRequest { Name = "Test", Language = "English" };
            var expectedShows = new List<Show> { new Show { Id = 1, Name = "Test Show", Language = "English" } };
            _mockShowRepository.Setup(r => r.GetFilteredShowsAsync(filters)).ReturnsAsync(expectedShows);

            var result = await _showService.GetFilteredShowsAsync(filters);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().Id);
            Assert.AreEqual("Test Show", result.First().Name);
        }
    }
}
