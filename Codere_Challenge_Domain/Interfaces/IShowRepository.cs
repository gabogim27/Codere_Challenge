using Codere_Challenge_Domain.Entities;

namespace Codere_Challenge_Core.Interfaces
{
    public interface IShowRepository
    {
        Task<IEnumerable<Show>> GetAllAsync();
        Task<Show> GetByIdAsync(int id);
        Task AddAsync(Show show);
    }
}
