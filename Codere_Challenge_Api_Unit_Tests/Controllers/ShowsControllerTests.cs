using Codere_Challenge_Api.Controllers;
using Codere_Challenge_Core.Filters;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Codere_Challenge_Api_Unit_Tests.Controllers
{
    [TestFixture]
    public class ShowsControllerTests
    {
        private Mock<ILogger<ShowsController>> _mockLogger;
        private Mock<IShowService> _mockShowService;
        private ShowsController _showsController;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<ShowsController>>();
            _mockShowService = new Mock<IShowService>();
            _showsController = new ShowsController(_mockLogger.Object, _mockShowService.Object);
        }

        [Test]
        public async Task GetFilteredShowsTest()
        {
            var filters = new ShowFilterRequest { Name = "Test", Language = "English" };
            var shows = new List<Show>
            {
                new Show { Id = 1, Name = "Test Show", Language = "English" }
            };
            _mockShowService.Setup(s => s.GetFilteredShowsAsync(filters)).ReturnsAsync(shows);
            var result = await _showsController.GetFilteredShows(filters);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.EqualTo(shows));
        }
    }
}
