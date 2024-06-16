using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Infrastructure.Data;

namespace Codere_Challenge_Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TvMazeDbContext _context;
        private ShowRepository _shows;
        private NetworkRepository _networks;
        private JobExecutionRepository _jobExecutionRepository;

        public UnitOfWork(TvMazeDbContext context, INetworkRepository networkRepository)
        {
            _context = context;
            _networks = networkRepository as NetworkRepository;
        }

        public INetworkRepository Networks => _networks ??= new NetworkRepository(_context);
        public IShowRepository Shows => _shows ??= new ShowRepository(_context, _networks);
        public IJobExecutionRepository JobExecution => _jobExecutionRepository ??= new JobExecutionRepository(_context);

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
