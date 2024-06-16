using Codere_Challenge_Core.Entities;
using Codere_Challenge_Domain.Entities;

namespace Codere_Challenge_Core.Interfaces
{
    public interface INetworkRepository
    {
        Task<Network> GetByIdAsync(int id);
        Task AddAsync(Network network);
        Task UpdateAsync(Network network);
    }
}
