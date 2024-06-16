using Codere_Challenge_Core.Filters;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Services.Interfaces;

namespace Codere_Challenge_Services.Implementations
{
    public class ShowService : IShowService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Show>> GetShowsAsync()
        {
            return await _unitOfWork.Shows.GetAllAsync();
        }

        public async Task<Show> GetShowByIdAsync(int id)
        {
            return await _unitOfWork.Shows.GetByIdAsync(id);
        }

        public async Task<List<Show>> GetShowByListOfIdsAsync(List<int> ids)
        {
            return await _unitOfWork.Shows.GetByListOfIdsAsync(ids);
        }

        public async Task AddShowAsync(Show show)
        {
            await AddToShows(show);
        }

        public async Task UpdateShowsAsync(List<Show> shows)
        {
            await _unitOfWork.Shows.UpdateRangeAsync(shows);
            await _unitOfWork.CompleteAsync();
        }

        private async Task AddToShows(Show show)
        {
            await _unitOfWork.Shows.AddAsync(show);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Show>> GetFilteredShowsAsync(ShowFilterRequest filters)
        {
            return await _unitOfWork.Shows.GetFilteredShowsAsync(filters);
        }
    }
}
