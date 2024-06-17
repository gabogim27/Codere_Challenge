using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Codere_Challenge_Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing job execution statuses in the database.
    /// </summary>
    public class JobExecutionRepository : IJobExecutionRepository
    {
        private readonly TvMazeDbContext _context;

        public JobExecutionRepository(TvMazeDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a job execution status by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the job execution status.</param>
        /// <returns>The job execution status with the specified ID.</returns>
        public async Task<JobExecutionStatus> GetByIdAsync(int id)
        {
            return await _context.JobExecutionStatus.FindAsync(id);
        }

        /// <summary>
        /// Gets all job execution statuses asynchronously.
        /// </summary>
        /// <returns>A list of all job execution statuses.</returns>
        public async Task<List<JobExecutionStatus>> GetAllAsync()
        {
            return await _context.JobExecutionStatus.ToListAsync();
        }

        /// <summary>
        /// Adds a new job execution status asynchronously.
        /// </summary>
        /// <param name="network">The job execution status to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync(JobExecutionStatus network)
        {
            await _context.JobExecutionStatus.AddAsync(network);
        }

        /// <summary>
        /// Updates an existing job execution status asynchronously.
        /// </summary>
        /// <param name="network">The job execution status to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAsync(JobExecutionStatus network)
        {
            _context.JobExecutionStatus.Update(network);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets the latest job execution status asynchronously.
        /// </summary>
        /// <returns>The latest job execution status.</returns>
        public Task<JobExecutionStatus?> GetLatestJob()
        {
            var lastJob = _context.JobExecutionStatus.OrderByDescending(x => x.ExecutionDate)?.FirstOrDefault();
            return Task.FromResult(lastJob);
        }
    }
}
