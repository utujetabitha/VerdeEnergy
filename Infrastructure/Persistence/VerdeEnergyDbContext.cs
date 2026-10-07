using Core.Domain.Entities;
using System.Globalization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence;

public sealed class VerdeEnergyDbContext(DbContextOptions<VerdeEnergyDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<TripRequest> TripRequests => Set<TripRequest>();
    public DbSet<HubSession> HubSessions => Set<HubSession>();
    public DbSet<InstallationQuote> InstallationQuotes => Set<InstallationQuote>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<ConsultingProject> ConsultingProjects => Set<ConsultingProject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var legacyDateTime = new ValueConverter<DateTime, string>(
            value => value.ToString("O", CultureInfo.InvariantCulture),
            value => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Vehicle>().ToTable("Vehicles");
        modelBuilder.Entity<TripRequest>().ToTable("TripRequests");
        modelBuilder.Entity<HubSession>().ToTable("HubSessions");
        modelBuilder.Entity<InstallationQuote>().ToTable("InstallationQuotes");
        modelBuilder.Entity<Transaction>().ToTable("Transactions");
        modelBuilder.Entity<ConsultingProject>().ToTable("ConsultingProjects");
        modelBuilder.Entity<TripRequest>().Property(trip => trip.CustomerUserId).HasMaxLength(450);
        modelBuilder.Entity<Transaction>().Property(transaction => transaction.OwnerUserId).HasMaxLength(450);
        modelBuilder.Entity<TripRequest>().Property(trip => trip.PickupTime).HasConversion(legacyDateTime).HasColumnType("nvarchar(50)");
        modelBuilder.Entity<TripRequest>().Property(trip => trip.ReturnTime).HasConversion(legacyDateTime).HasColumnType("nvarchar(50)");
        modelBuilder.Entity<HubSession>().Property(session => session.ChargingDuration).HasConversion(legacyDateTime).HasColumnType("nvarchar(50)");
        modelBuilder.Entity<ConsultingProject>().Property(project => project.CompletionDate).HasConversion(legacyDateTime).HasColumnType("nvarchar(50)");

    }
}