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

        public async Task AddShowAsync(Show show)
        {
            await _unitOfWork.Shows.AddAsync(show);
            await _unitOfWork.CompleteAsync();
        }
    }
}
