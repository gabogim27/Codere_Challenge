using System.Net.Http.Json;
using Codere_Challenge_Core.Settings;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Services.Implementations;
using Codere_Challenge_Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Codere_Challenge_Jobs.Jobs
{
    public class FetchShowsJob : IFetchShowsJob
    {
        private readonly IShowService _showService;

        private readonly JobSettings _jobSettings;

        private readonly HttpClient _httpClient;

        public FetchShowsJob(IShowService showService, HttpClient httpClient, IOptions<JobSettings> settings)
        {
            _showService = showService;
            _httpClient = httpClient;
            _jobSettings = settings.Value;
        }
        public async Task ExecuteAsync()
        {
            var page = 1;
            var shows = new List<Show>();

            do
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<Show>>($"{_jobSettings.TvMazeApiUrl}{page}");
                shows = response?.ToList();

                foreach (var show in shows)
                {
                    await _showService.AddShowAsync(show);
                }

                page++;
            } while (shows.Count == _jobSettings.BatchSize);
        }
    }
}
