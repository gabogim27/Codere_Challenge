using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Codere_Challenge_Infrastructure.Repositories
{
    public class NetworkRepository : INetworkRepository
    {
        private readonly TvMazeDbContext _context;

        public NetworkRepository(TvMazeDbContext context)
        {
            _context = context;
        }

        public async Task<Network> GetByIdAsync(int id)
        {
            return await _context.Networks.FindAsync(id);
        }

        public async Task AddAsync(Network network)
        {
            await _context.Networks.AddAsync(network);
        }

        public async Task UpdateAsync(Network network)
        {
            _context.Networks.Update(network);
            await Task.CompletedTask;
        }
    }
}
