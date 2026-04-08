using JetLag.Scripts.Data.Gtfs;
using Microsoft.EntityFrameworkCore;

namespace JetLag.Scripts.Data;

public class GtfsDbContext : DbContext
{
    public GtfsDbContext(DbContextOptions<GtfsDbContext> options) : base(options) { }

    public DbSet<GtfsStop> Stops => Set<GtfsStop>();
    public DbSet<GtfsRoute> Routes => Set<GtfsRoute>();
    public DbSet<GtfsTrip> Trips => Set<GtfsTrip>();
    public DbSet<GtfsStopTime> StopTimes => Set<GtfsStopTime>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GtfsStop>().HasKey(e => e.StopId);
        modelBuilder.Entity<GtfsRoute>().HasKey(e => e.RouteId);

        modelBuilder.Entity<GtfsTrip>().HasKey(e => e.TripId);
        modelBuilder.Entity<GtfsTrip>()
            .HasOne(t => t.Route)
            .WithMany(r => r.Trips)
            .HasForeignKey(t => t.RouteId);

        modelBuilder.Entity<GtfsStopTime>().HasKey(e => e.Id);
        modelBuilder.Entity<GtfsStopTime>()
            .HasOne(st => st.Trip)
            .WithMany(t => t.StopTimes)
            .HasForeignKey(st => st.TripId);
        modelBuilder.Entity<GtfsStopTime>()
            .HasOne(st => st.Stop)
            .WithMany(s => s.StopTimes)
            .HasForeignKey(st => st.StopId);
        modelBuilder.Entity<GtfsStopTime>()
            .HasIndex(st => new { st.StopId, st.DepartureSeconds });
    }
}
