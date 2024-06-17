using Codere_Challenge_Core.Filters;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Services.Interfaces;

namespace Codere_Challenge_Services.Implementations
{
    /// <summary>
    /// Service for managing show-related operations.
    /// </summary>
    public class ShowService : IShowService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all shows asynchronously.
        /// </summary>
        /// <returns>A list of all shows.</returns>
        public async Task<IEnumerable<Show>> GetShowsAsync()
        {
            return await _unitOfWork.Shows.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a show by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the show.</param>
        /// <returns>The show with the specified ID.</returns>
        public async Task<Show> GetShowByIdAsync(int id)
        {
            return await _unitOfWork.Shows.GetByIdAsync(id);
        }

        /// <summary>
        /// Retrieves shows by a list of IDs asynchronously.
        /// </summary>
        /// <param name="ids">The list of show IDs.</param>
        /// <returns>A list of shows with the specified IDs.</returns>
        public async Task<List<Show>> GetShowByListOfIdsAsync(List<int> ids)
        {
            return await _unitOfWork.Shows.GetByListOfIdsAsync(ids);
        }

        /// <summary>
        /// Adds a new show asynchronously.
        /// </summary>
        /// <param name="show">The show to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddShowAsync(Show show)
        {
            await AddToShows(show);
        }

        /// <summary>
        /// Updates a list of shows asynchronously.
        /// </summary>
        /// <param name="shows">The list of shows to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateShowsAsync(List<Show> shows)
        {
            await _unitOfWork.Shows.UpdateRangeAsync(shows);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Adds a new show to the database asynchronously.
        /// </summary>
        /// <param name="show">The show to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddToShows(Show show)
        {
            await _unitOfWork.Shows.AddAsync(show);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Retrieves filtered shows based on provided filter criteria asynchronously.
        /// </summary>
        /// <param name="filters">The filter criteria for shows.</param>
        /// <returns>A list of shows matching the filter criteria.</returns>
        public async Task<IEnumerable<Show>> GetFilteredShowsAsync(ShowFilterRequest filters)
        {
            return await _unitOfWork.Shows.GetFilteredShowsAsync(filters);
        }
    }
}
