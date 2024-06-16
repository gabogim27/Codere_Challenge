using Codere_Challenge_Core.Entities;

namespace Codere_Challenge_Services.Interfaces
{
    public interface IJobExecutionService
    {
        Task<JobExecutionStatus> GetJobStatusByIdAsync(int id);
        Task AddOrUpdateJobStatusAsync(JobExecutionStatus jobStatus);
        Task<JobExecutionStatus?> GetLatestJob();
    }
}
