using Codere_Challenge_Core.Filters;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Codere_Challenge_Api.Controllers
{
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

        [HttpPost("Filter")]
        public async Task<ActionResult<IEnumerable<Show>>> GetFilteredShows([FromBody] ShowFilterRequest filters)
        {
            var shows = await _showService.GetFilteredShowsAsync(filters);
            return Ok(shows);
        }
    }
}
