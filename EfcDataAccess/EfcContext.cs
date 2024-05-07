using System.Diagnostics.Metrics;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess;

public class EfcContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Measurement> Measurements { get; set; }

  

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source = ../EfcDataAccess/Greenhouse.db");
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(user => user.Id);
        modelBuilder.Entity<Measurement>()
            .HasKey(m => m.Id);  // Explicitly setting Id as the primary key

        modelBuilder.Entity<Measurement>()
            .HasDiscriminator<string>("Type")
            .HasValue<Temperature>("Temperature")
            .HasValue<Humidity>("Humidity");
        modelBuilder.Entity<Measurement>()
            .Property(m => m.Time)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}