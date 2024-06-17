using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Services.Interfaces;

namespace Codere_Challenge_Services.Implementations
{
    /// <summary>
    /// Service for managing job execution statuses.
    /// </summary>
    public class JobExecutionService : IJobExecutionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JobExecutionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves a job execution status by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the job execution status.</param>
        /// <returns>The job execution status with the specified ID.</returns>
        public async Task<JobExecutionStatus> GetJobStatusByIdAsync(int id)
        {
            return await _unitOfWork.JobExecution.GetByIdAsync(id);
        }

        /// <summary>
        /// Retrieves all job execution statuses asynchronously.
        /// </summary>
        /// <returns>A list of all job execution statuses.</returns>
        public async Task<List<JobExecutionStatus>> GetAllAsync()
        {
            return await _unitOfWork.JobExecution.GetAllAsync();
        }

        /// <summary>
        /// Adds or updates a job execution status asynchronously.
        /// </summary>
        /// <param name="jobStatus">The job execution status to add or update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Retrieves the latest job execution status asynchronously.
        /// </summary>
        /// <returns>The latest job execution status.</returns>
        public async Task<JobExecutionStatus?> GetLatestJob()
        {
            return await _unitOfWork.JobExecution.GetLatestJob();
        }
    }
}
