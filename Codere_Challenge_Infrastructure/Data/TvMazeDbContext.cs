using Codere_Challenge_Core.Entities;
using Codere_Challenge_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Codere_Challenge_Infrastructure.Data
{
    public class TvMazeDbContext : DbContext
    {
        public TvMazeDbContext(DbContextOptions<TvMazeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<Network> Networks { get; set; }
        public DbSet<JobExecutionStatus> JobExecutionStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Show entity and its properties
            modelBuilder.Entity<Show>(entity =>
            {
                entity.Property(e => e.Name).IsRequired(false);
                entity.Property(e => e.Summary).IsRequired(false);
                entity.Property(e => e.Url).IsRequired(false);
                entity.Property(e => e.Type).IsRequired(false);
                entity.Property(e => e.Language).IsRequired(false);
                entity.Property(e => e.Status).IsRequired(false);
                entity.Property(e => e.Runtime).IsRequired(false);
                entity.Property(e => e.AverageRuntime).IsRequired(false);
                entity.Property(e => e.Premiered).IsRequired(false);
                entity.Property(e => e.Ended).IsRequired(false);
                entity.Property(e => e.OfficialSite).IsRequired(false);
                entity.Property(e => e.Weight).IsRequired(false);
                entity.Property(e => e.Updated).IsRequired(false);

                var valueComparer = new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                );

                entity.Property(e => e.Genres)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(valueComparer);
            });

            modelBuilder.Entity<Show>().OwnsOne(s => s.Schedule, schedule =>
            {
                schedule.Property(s => s.Time).IsRequired(false);
                schedule.Property(s => s.Days).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()).Metadata.SetValueComparer(
                        new ValueComparer<List<string>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()
                        )
                    );
            });

            modelBuilder.Entity<Show>().OwnsOne(s => s.Rating, rating =>
            {
                rating.Property(r => r.Average).IsRequired(false);
            });

            modelBuilder.Entity<Show>().HasOne(s => s.Network)
                .WithMany()
                .HasForeignKey("NetworkId")
                .IsRequired(false);

            // Configure Network entity
            modelBuilder.Entity<Network>().OwnsOne(n => n.Country, country =>
            {
                country.Property(c => c.Name).IsRequired(false);
                country.Property(c => c.Code).IsRequired(false);
                country.Property(c => c.Timezone).IsRequired(false);
            });

            // Configure JobExecutionStatus entity
            modelBuilder.Entity<JobExecutionStatus>(entity =>
            {
                entity.Property(e => e.JobStatus).HasConversion<int>();
                entity.Property(e => e.ExecutionDate).IsRequired();
                entity.Property(e => e.LastIndexProcessed).IsRequired();
            });
        }
    }
}
