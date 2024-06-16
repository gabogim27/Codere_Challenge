using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Codere_Challenge_Infrastructure.Repositories
{
    public class JobExecutionRepository : IJobExecutionRepository
    {
        private readonly TvMazeDbContext _context;

        public JobExecutionRepository(TvMazeDbContext context)
        {
            _context = context;
        }

        public async Task<JobExecutionStatus> GetByIdAsync(int id)
        {
            return await _context.JobExecutionStatus.FindAsync(id);
        }

        public async Task<List<JobExecutionStatus>> GetAllAsync()
        {
            return await _context.JobExecutionStatus.ToListAsync();
        }

        public async Task AddAsync(JobExecutionStatus network)
        {
            await _context.JobExecutionStatus.AddAsync(network);
        }

        public async Task UpdateAsync(JobExecutionStatus network)
        {
            _context.JobExecutionStatus.Update(network);
            await Task.CompletedTask;
        }

        public Task<JobExecutionStatus?> GetLatestJob()
        {
            var lastJob = _context.JobExecutionStatus.OrderByDescending(x => x.ExecutionDate)?.FirstOrDefault();
            return Task.FromResult(lastJob);
        }
    }
}
