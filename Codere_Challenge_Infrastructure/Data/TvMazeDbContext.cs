using Codere_Challenge_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codere_Challenge_Infrastructure.Data
{
    public class TvMazeDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public TvMazeDbContext(DbContextOptions<TvMazeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Show> Shows { get; set; }
    }
}
