using Codere_Challenge_Core.Entities;

namespace Codere_Challenge_Core.Interfaces
{
    public interface IJobExecutionRepository
    {
        Task<JobExecutionStatus> GetByIdAsync(int id);
        Task<List<JobExecutionStatus>> GetAllAsync();
        Task AddAsync(JobExecutionStatus network);
        Task UpdateAsync(JobExecutionStatus network);
        Task<JobExecutionStatus?> GetLatestJob();
    }
}
