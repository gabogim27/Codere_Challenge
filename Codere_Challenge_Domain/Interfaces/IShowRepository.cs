using Codere_Challenge_Core.Filters;
using Codere_Challenge_Domain.Entities;

namespace Codere_Challenge_Core.Interfaces
{
    public interface IShowRepository
    {
        Task<IEnumerable<Show>> GetAllAsync();
        Task<Show> GetByIdAsync(int id);
        Task AddAsync(Show show);
        Task AddRangeAsync(List<Show> shows);
        Task<List<Show>> GetByListOfIdsAsync(List<int> ids);
        Task UpdateRangeAsync(List<Show> shows);
        Task<IEnumerable<Show>> GetFilteredShowsAsync(ShowFilterRequest filters);
    }
}
