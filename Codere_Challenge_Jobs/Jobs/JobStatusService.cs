namespace Codere_Challenge_Jobs.Jobs
{
    public class JobStatusService : IJobStatusService
    {
        public static bool _isJobRunning;

        public bool IsJobRunning => _isJobRunning;

        public void SetJobRunning(bool isRunning)
        {
            _isJobRunning = isRunning;
        }
    }
}
