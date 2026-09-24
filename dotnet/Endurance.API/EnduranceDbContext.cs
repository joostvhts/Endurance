using Endurance.API.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Endurance.API;

public class EnduranceDbContext : DbContext
{
    public DbSet<NotifierSettingsEntity> NotifierSettings { get; set; }
    public DbSet<WatchedClassEntity> WatchedClasses { get; set; }
    public DbSet<VerifiedEmailEntity> VerifiedEmails { get; set; }

    public EnduranceDbContext(DbContextOptions<EnduranceDbContext> options) 
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotifierSettingsEntity>(entity =>
        {
            entity.Property(e => e.Settings).HasColumnType("json");
        });

        modelBuilder.Entity<VerifiedEmailEntity>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.VerificationToken).IsUnique();
            entity.HasIndex(e => e.UnsubscribeToken).IsUnique();
        });

        // Ensure that the model configurations match the existing database schema
        base.OnModelCreating(modelBuilder);
    }
}
