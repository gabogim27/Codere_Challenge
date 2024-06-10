using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Codere_Challenge_Infrastructure.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly TvMazeDbContext _context;

        public ShowRepository(TvMazeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Show>> GetAllAsync()
        {
            return await _context.Shows.ToListAsync();
        }

        public async Task<Show> GetByIdAsync(int id)
        {
            return await _context.Shows.FindAsync(id);
        }

        public async Task AddAsync(Show show)
        {
            await _context.Shows.AddAsync(show);
        }
    }
}
