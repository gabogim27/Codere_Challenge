namespace Codere_Challenge_Jobs.Jobs
{
    public interface IJobStatusService
    {
        bool IsJobRunning { get; }

        void SetJobRunning(bool isRunning); 
    }
}
