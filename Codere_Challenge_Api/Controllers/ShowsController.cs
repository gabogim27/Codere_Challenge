using Codere_Challenge_Core.Filters;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Codere_Challenge_Api.Controllers
{
    /// <summary>
    /// Controller for managing show-related operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ILogger<ShowsController> _logger;
        private readonly IShowService _showService;

        public ShowsController(ILogger<ShowsController> logger, IShowService showService)
        {
            _logger = logger;
            _showService = showService;
        }

        /// <summary>
        /// Retrieves filtered shows based on provided filter criteria.
        /// </summary>
        /// <param name="filters">The filter criteria for shows.</param>
        /// <returns>A list of shows matching the filter criteria.</returns>
        [HttpPost("Filter")]
        public async Task<ActionResult<IEnumerable<Show>>> GetFilteredShows([FromBody] ShowFilterRequest filters)
        {
            var shows = await _showService.GetFilteredShowsAsync(filters);
            return Ok(shows);
        }
    }
}
