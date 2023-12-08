using Microsoft.EntityFrameworkCore;
using Space_Weather.Models;

namespace Space_Weather.Data
{
    public class SpaceWeatherContext : DbContext
    {
        public SpaceWeatherContext(DbContextOptions<SpaceWeatherContext> options) : base(options) { }

        public DbSet<Planet> Planets { get; set; }
        public DbSet<Weather> Weathers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Planet and Weather
            modelBuilder.Entity<Planet>()
                .HasMany(p => p.Weathers)
                .WithOne(w => w.Planet)
                .HasForeignKey(w => w.PlanetId);

            // Add an index to the Date field in Weather
            modelBuilder.Entity<Weather>()
                .HasIndex(w => w.Date);

            // Data validation rules (optional)
            modelBuilder.Entity<Weather>()
                .Property(w => w.Forecast)
                .HasMaxLength(100);

        }
    }
}
