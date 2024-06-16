using Codere_Challenge_Core.Filters;
using Codere_Challenge_Domain.Entities;

namespace Codere_Challenge_Services.Interfaces
{
    public interface IShowService
    {
        Task<IEnumerable<Show>> GetShowsAsync();
        Task<Show> GetShowByIdAsync(int id);
        Task AddShowAsync(Show show);
        Task<List<Show>> GetShowByListOfIdsAsync(List<int> ids);
        Task UpdateShowsAsync(List<Show> shows);
        Task<IEnumerable<Show>> GetFilteredShowsAsync(ShowFilterRequest filters);
    }
}
