using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Settings;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Services.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Codere_Challenge_Jobs.Jobs
{
    public class FetchShowsJob : IFetchShowsJob
    {
        private readonly IShowService _showService;
        private readonly JobSettings _jobSettings;
        private readonly HttpClient _httpClient;
        private readonly IJobStatusService _jobStatusService;
        private readonly IJobExecutionService _jobExecutionService;
        private readonly ILogger<FetchShowsJob> _logger;

        public FetchShowsJob(ILogger<FetchShowsJob> logger, IShowService showService, HttpClient httpClient, IOptions<JobSettings> settings, IJobStatusService jobStatusService, IJobExecutionService jobExecutionService)
        {
            _logger = logger;
            _showService = showService;
            _httpClient = httpClient;
            _jobSettings = settings.Value;
            _jobStatusService = jobStatusService;
            _jobExecutionService = jobExecutionService;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        public async Task ExecuteAsync()
        {
            if (_jobStatusService.IsJobRunning)
            {
                throw new InvalidOperationException("Job is already running.");
            }

            _jobStatusService.SetJobRunning(true);
            JobExecutionStatus currentJobInstance = null;
            var lastJobProcessed = await _jobExecutionService.GetLatestJob();

            try
            {
                currentJobInstance = ProcessJobStatus(lastJobProcessed, currentJobInstance);
                var batchSize = _jobSettings.BatchSize > 0 ? _jobSettings.BatchSize : 50;
                var totalShows = await FetchShowsAsync();
                var lastIdProcessed = currentJobInstance?.LastIndexProcessed;

                if (totalShows?.Count > 0)
                {
                    var showsToPorocess = GetPage(totalShows, lastIdProcessed, batchSize);
                    while (showsToPorocess.Count > 0)
                    {
                        var showsInDatabase = await UpdateExistingShows(showsToPorocess);
                        await AddNewShows(showsToPorocess, showsInDatabase);

                        lastIdProcessed = showsToPorocess.Last()?.Id;
                        lastIdProcessed++;
                        showsToPorocess = GetPage(totalShows, lastIdProcessed, batchSize);
                    }

                    UpdateJobExecutionStatus(lastIdProcessed.Value-1, JobStatus.FINISHED, currentJobInstance);
                }

                _logger.LogDebug("Job finished successfully.");
            }
            catch (Exception ex)
            {
                UpdateJobExecutionStatus(null, JobStatus.FAILED, currentJobInstance);
                _logger.LogError("An error occured when processing the job. Error: ", ex);
            }
            finally
            {
                _jobStatusService.SetJobRunning(false);
            }
        }

        private JobExecutionStatus? ProcessJobStatus(JobExecutionStatus? lastJobProcessed, JobExecutionStatus? currentJobInstance)
        {
            if (lastJobProcessed == null || lastJobProcessed?.JobStatus == JobStatus.FINISHED)
            {
                var newJobObj = new JobExecutionStatus();
                UpdateJobExecutionStatus(null, JobStatus.INPROCESS, newJobObj);
                currentJobInstance = newJobObj;
            }

            if (lastJobProcessed?.JobStatus == JobStatus.INPROCESS || lastJobProcessed?.JobStatus == JobStatus.FAILED)
            {
                currentJobInstance = lastJobProcessed;
            }

            return currentJobInstance;
        }

        private async Task AddNewShows(List<Show> showsToPorocess, List<Show>? showsInDatabase)
        {
            var showsToInsert = showsToPorocess.Where(x => showsInDatabase != null && showsInDatabase.All(y => y.Id != x.Id)).ToList();
            if (showsToInsert.Count > 0)
            {
                foreach (var show in showsToInsert)
                {
                    await _showService.AddShowAsync(show);
                }
            }
        }

        private async Task<List<Show>?> UpdateExistingShows(List<Show> showsToPorocess)
        {
            var showsInDatabase =
                _showService.GetShowByListOfIdsAsync(showsToPorocess.Select(x => x.Id).ToList())?.Result;
            if (showsInDatabase?.Count > 0)
            {
                await _showService.UpdateShowsAsync(showsInDatabase);
            }

            return showsInDatabase;
        }

        private async void UpdateJobExecutionStatus(int? idProcessed, JobStatus status, JobExecutionStatus currentInstance)
        {
            currentInstance.JobStatus = status;
            currentInstance.LastIndexProcessed = idProcessed.HasValue ? idProcessed.Value : 0;
            currentInstance.ExecutionDate = DateTime.UtcNow;
            await _jobExecutionService.AddOrUpdateJobStatusAsync(currentInstance);
        }

        public List<Show> GetPage(List<Show> shows, int? lastIdProcessed, int batchSize)
        {
            if (lastIdProcessed.HasValue && lastIdProcessed.Value == 0)
            {
                lastIdProcessed++;
            }

            return shows.Where(x => x.Id >= lastIdProcessed).Take(batchSize).ToList();
        }

        private async Task<List<Show>?> FetchShowsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_jobSettings.TvMazeApiUrl);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var shows = JsonSerializer.Deserialize<List<Show>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return shows;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occured when fetching shows from: {_jobSettings.TvMazeApiUrl}. Error: ", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured when fetching shows from: {_jobSettings.TvMazeApiUrl}. Error: ", ex);
                throw;
            }
        }
    }
}
