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
    /// <summary>
    /// Job for fetching and storing shows from an external API.
    /// </summary>
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

        /// <summary>
        /// Executes the job to fetch and store shows.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Processes the job status based on the last job processed.
        /// </summary>
        /// <param name="lastJobProcessed">The last job processed.</param>
        /// <param name="currentJobInstance">The current job instance.</param>
        /// <returns>The processed job status.</returns>
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

        /// <summary>
        /// Adds new shows to the database.
        /// </summary>
        /// <param name="showsToPorocess">The shows to process.</param>
        /// <param name="showsInDatabase">The shows in the database.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Updates existing shows in the database.
        /// </summary>
        /// <param name="showsToPorocess">The shows to process.</param>
        /// <returns>A list of shows in the database.</returns>
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

        /// <summary>
        /// Updates the job execution status.
        /// </summary>
        /// <param name="idProcessed">The ID of the last processed show.</param>
        /// <param name="status">The job status.</param>
        /// <param name="currentInstance">The current job instance.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async void UpdateJobExecutionStatus(int? idProcessed, JobStatus status, JobExecutionStatus currentInstance)
        {
            currentInstance.JobStatus = status;
            currentInstance.LastIndexProcessed = idProcessed.HasValue ? idProcessed.Value : 0;
            currentInstance.ExecutionDate = DateTime.UtcNow;
            await _jobExecutionService.AddOrUpdateJobStatusAsync(currentInstance);
        }

        /// <summary>
        /// Retrieves a page of shows to process.
        /// </summary>
        /// <param name="shows">The list of shows.</param>
        /// <param name="lastIdProcessed">The ID of the last processed show.</param>
        /// <param name="batchSize">The batch size.</param>
        /// <returns>A list of shows to process.</returns>
        public List<Show> GetPage(List<Show> shows, int? lastIdProcessed, int batchSize)
        {
            if (lastIdProcessed.HasValue && lastIdProcessed.Value == 0)
            {
                lastIdProcessed++;
            }

            return shows.Where(x => x.Id >= lastIdProcessed).Take(batchSize).ToList();
        }

        /// <summary>
        /// Fetches shows from the external API.
        /// </summary>
        /// <returns>A list of shows.</returns>
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
