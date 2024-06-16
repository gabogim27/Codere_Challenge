using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Services.Interfaces;

namespace Codere_Challenge_Services.Implementations
{
    public class JobExecutionService : IJobExecutionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JobExecutionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<JobExecutionStatus> GetJobStatusByIdAsync(int id)
        {
            return await _unitOfWork.JobExecution.GetByIdAsync(id);
        }

        public async Task<List<JobExecutionStatus>> GetAllAsync()
        {
            return await _unitOfWork.JobExecution.GetAllAsync();
        }

        public async Task AddOrUpdateJobStatusAsync(JobExecutionStatus jobStatus)
        {
            var jobExecutionStatus = _unitOfWork.JobExecution.GetByIdAsync(jobStatus.Id);
            if (jobExecutionStatus.Result != null)
            {
                await _unitOfWork.JobExecution.UpdateAsync(jobStatus);
            }
            else
            {
                await _unitOfWork.JobExecution.AddAsync(jobStatus);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<JobExecutionStatus?> GetLatestJob()
        {
            return await _unitOfWork.JobExecution.GetLatestJob();
        }
    }
}
