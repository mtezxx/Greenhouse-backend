using System.Diagnostics.Metrics;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess;

public class EfcContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Measurement> Measurements { get; set; }

    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Threshold> Thresholds { get; set; }
    public DbSet<EmailNotification> EmailNotifications { get; set; }
    public EfcContext() : base() { } 

    public EfcContext(DbContextOptions<EfcContext> options) : base(options)
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("DBDIR")))
        {
            optionsBuilder.UseSqlite("Data Source = ../EfcDataAccess/Greenhouse.db");
        }
        else
        {
            optionsBuilder.UseSqlite("Data Source = "+Environment.GetEnvironmentVariable("DBDIR")+"/Greenhouse.db");
        }
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(user => user.Id);
        modelBuilder.Entity<Measurement>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<Measurement>()
            .HasDiscriminator<string>("Type")
            .HasValue<Temperature>("Temperature")
            .HasValue<Humidity>("Humidity")
            .HasValue<Light>("Light");
        modelBuilder.Entity<Measurement>()
            .Property(m => m.Time)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<Notification>().HasKey(n => n.Id);
        modelBuilder.Entity<Threshold>().HasKey(n => n.Id);
        modelBuilder.Entity<EmailNotification>(entity =>
        {
            entity.HasKey(e => e.Email);
            entity.Ignore(e => e.Title);
            entity.Ignore(e => e.Body);
        });
    }
}