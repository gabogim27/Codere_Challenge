using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Infrastructure.Data;

namespace Codere_Challenge_Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TvMazeDbContext _context;
        private ShowRepository _shows;

        public UnitOfWork(TvMazeDbContext context)
        {
            _context = context;
        }

        public IShowRepository Shows => _shows ??= new ShowRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
