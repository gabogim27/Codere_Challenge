using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Infrastructure.Data;

namespace Codere_Challenge_Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing network data in the database.
    /// </summary>
    public class NetworkRepository : INetworkRepository
    {
        private readonly TvMazeDbContext _context;

        public NetworkRepository(TvMazeDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a network by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the network.</param>
        /// <returns>The network with the specified ID.</returns>
        public async Task<Network> GetByIdAsync(int id)
        {
            return await _context.Networks.FindAsync(id);
        }

        /// <summary>
        /// Adds a new network asynchronously.
        /// </summary>
        /// <param name="network">The network to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync(Network network)
        {
            await _context.Networks.AddAsync(network);
        }

        /// <summary>
        /// Updates an existing network asynchronously.
        /// </summary>
        /// <param name="network">The network to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAsync(Network network)
        {
            _context.Networks.Update(network);
            await Task.CompletedTask;
        }
    }
}
