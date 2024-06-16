using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Codere_Challenge_Infrastructure.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly TvMazeDbContext _context;
        private readonly INetworkRepository _networkRepository;

        public ShowRepository(TvMazeDbContext context, INetworkRepository networkRepository)
        {
            _context = context;
            _networkRepository = networkRepository;
            //_networkRepository = networkRepository ?? throw new ArgumentNullException(nameof(networkRepository));
        }

        public async Task<IEnumerable<Show>> GetAllAsync()
        {
            return await _context.Shows.ToListAsync();
        }

        public async Task<Show> GetByIdAsync(int id)
        {
            return await _context.Shows.FindAsync(id);
        }

        public async Task<List<Show>> GetByListOfIdsAsync(List<int> ids)
        {
            return await _context.Shows.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task AddAsync(Show show)
        {
            await EnsureNetworkTrackedAsync(show.Network);
            await _context.Shows.AddAsync(show);
        }

        public async Task AddRangeAsync(List<Show> shows)
        {
            foreach (var show in shows)
            {
                await EnsureNetworkTrackedAsync(show.Network);
            }

            await _context.Shows.AddRangeAsync(shows);
        }

        public async Task UpdateRangeAsync(List<Show> shows)
        {
            foreach (var show in shows)
            {
                await EnsureNetworkTrackedAsync(show.Network);
            }
            _context.Shows.UpdateRange(shows);
            await Task.CompletedTask;
        }

        public Task<int> GetMaxId()
        {
            var maxId = _context.Shows.Max(x => x.Id);
            return Task.FromResult(maxId);
        }

        private async Task EnsureNetworkTrackedAsync(Network network)
        {
            if (network != null)
            {
                var existingNetwork = await _context.Networks.FirstOrDefaultAsync(n => n.Id == network.Id);
                if (existingNetwork != null)
                {
                    _context.Entry(existingNetwork).State = EntityState.Detached;
                    _context.Entry(network).State = EntityState.Modified;
                }
                else
                {
                    await _context.Networks.AddAsync(network);
                    //if (!_context.Networks.Local.Contains(existingNetwork))
                    //{
                    //    _context.Networks.Attach(network);
                    //}
                }
            }
        }
    }
}
