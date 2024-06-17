using Codere_Challenge_Core.Entities;
using Codere_Challenge_Core.Filters;
using Codere_Challenge_Core.Interfaces;
using Codere_Challenge_Domain.Entities;
using Codere_Challenge_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Codere_Challenge_Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing show data in the database.
    /// </summary>
    public class ShowRepository : IShowRepository
    {
        private readonly TvMazeDbContext _context;

        public ShowRepository(TvMazeDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all shows asynchronously.
        /// </summary>
        /// <returns>A list of all shows.</returns>
        public async Task<IEnumerable<Show>> GetAllAsync()
        {
            return await _context.Shows.ToListAsync();
        }

        /// <summary>
        /// Gets a show by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the show.</param>
        /// <returns>The show with the specified ID.</returns>
        public async Task<Show> GetByIdAsync(int id)
        {
            return await _context.Shows.FindAsync(id);
        }

        /// <summary>
        /// Gets shows by a list of IDs asynchronously.
        /// </summary>
        /// <param name="ids">The list of show IDs.</param>
        /// <returns>A list of shows with the specified IDs.</returns>
        public async Task<List<Show>> GetByListOfIdsAsync(List<int> ids)
        {
            return await _context.Shows.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        /// <summary>
        /// Adds a new show asynchronously.
        /// </summary>
        /// <param name="show">The show to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync(Show show)
        {
            await EnsureNetworkTrackedAsync(show.Network);
            await _context.Shows.AddAsync(show);
        }

        /// <summary>
        /// Adds a range of new shows asynchronously.
        /// </summary>
        /// <param name="shows">The list of shows to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddRangeAsync(List<Show> shows)
        {
            foreach (var show in shows)
            {
                await EnsureNetworkTrackedAsync(show.Network);
            }

            await _context.Shows.AddRangeAsync(shows);
        }

        /// <summary>
        /// Updates a range of existing shows asynchronously.
        /// </summary>
        /// <param name="shows">The list of shows to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateRangeAsync(List<Show> shows)
        {
            foreach (var show in shows)
            {
                await EnsureNetworkTrackedAsync(show.Network);
            }
            _context.Shows.UpdateRange(shows);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets shows that match the specified filters asynchronously.
        /// </summary>
        /// <param name="filters">The filters to apply to the shows.</param>
        /// <returns>A list of shows that match the specified filters.</returns>
        public async Task<IEnumerable<Show>> GetFilteredShowsAsync(ShowFilterRequest filters)
        {
            var query = _context.Shows.Include(s => s.Schedule).Include(s => s.Rating).Include(s => s.Network).ThenInclude(n => n.Country).AsQueryable();

            if (!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(s => s.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Language))
            {
                query = query.Where(s => s.Language.ToLower() == filters.Language.ToLower());
            }

            if (!string.IsNullOrEmpty(filters.Status))
            {
                query = query.Where(s => s.Status.ToLower() == filters.Status.ToLower());
            }

            if (filters.PremieredAfter.HasValue)
            {
                query = query.Where(s => s.Premiered >= filters.PremieredAfter);
            }

            if (filters.PremieredBefore.HasValue)
            {
                query = query.Where(s => s.Premiered <= filters.PremieredBefore);
            }

            var shows = await query.ToListAsync();

            if (!string.IsNullOrEmpty(filters.Genre))
            {
                shows = shows.Where(s => s.Genres.Any(g => g.ToLower() == filters.Genre.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(filters.Summary))
            {
                shows = shows.Where(s => s.Summary != null && s.Summary.ToLower().Contains(filters.Summary.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(filters.Url))
            {
                shows = shows.Where(s => s.Url != null && s.Url.ToLower().Contains(filters.Url.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(filters.Type))
            {
                shows = shows.Where(s => s.Type.ToLower() == filters.Type.ToLower()).ToList();
            }

            if (filters.Runtime.HasValue)
            {
                shows = shows.Where(s => s.Runtime == filters.Runtime).ToList();
            }

            if (filters.AverageRuntime.HasValue)
            {
                shows = shows.Where(s => s.AverageRuntime == filters.AverageRuntime).ToList();
            }

            if (!string.IsNullOrEmpty(filters.OfficialSite))
            {
                shows = shows.Where(s => s.OfficialSite != null && s.OfficialSite.ToLower().Contains(filters.OfficialSite.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(filters.ScheduleTime))
            {
                shows = shows.Where(s => s.Schedule != null && s.Schedule.Time == filters.ScheduleTime).ToList();
            }

            if (filters.ScheduleDays != null && filters.ScheduleDays.Any())
            {
                shows = shows.Where(s => s.Schedule != null && filters.ScheduleDays.All(day => s.Schedule.Days.Contains(day))).ToList();
            }

            if (filters.RatingAverage.HasValue)
            {
                shows = shows.Where(s => s.Rating != null && s.Rating.Average == filters.RatingAverage).ToList();
            }

            if (filters.Weight.HasValue)
            {
                shows = shows.Where(s => s.Weight == filters.Weight).ToList();
            }

            if (!string.IsNullOrEmpty(filters.NetworkName))
            {
                shows = shows.Where(s => s.Network != null && s.Network.Name.ToLower().Contains(filters.NetworkName.ToLower())).ToList();
            }

            if (filters.UpdatedAfter.HasValue)
            {
                shows = shows.Where(s => s.Updated >= filters.UpdatedAfter.Value.Ticks).ToList();
            }

            return shows;
        }

        /// <summary>
        /// Ensures the network is tracked by the context.
        /// </summary>
        /// <param name="network">The network to track.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task EnsureNetworkTrackedAsync(Network network)
        {
            if (network != null)
            {
                var existingNetwork = await _context.Networks.FirstOrDefaultAsync(n => n.Id == network.Id);
                if (existingNetwork != null)
                {
                    _context.Entry(existingNetwork).State = EntityState.Detached;
                    _context.Entry(network).State = EntityState.Modified;
                }
                else
                {
                    await _context.Networks.AddAsync(network);
                }
            }
        }
    }
}
