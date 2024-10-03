using Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Database;

/// <summary>
/// EntityFramework application context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationContext"/> class.
/// </remarks>
/// <param name="options">Options for data-context.</param>
public class ApplicationContext(
    DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    private const string SchemaName = "identity";
    
    public DbSet<UserEntity> User { get; init; }
    
    public DbSet<LoginRecordEntity> LoginRecord { get; init; }
    
    public DbSet<RefreshRecordEntity> RefreshRecord { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(SchemaName);
        
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity
                .HasIndex(e => new { e.ProviderId, e.AuthenticationProvider })
                .IsUnique();
            
            entity
                .HasMany(o => o.LoginRecords);
        });

        modelBuilder.Entity<LoginRecordEntity>(entity =>
        {
            entity
                .HasOne(o => o.User)
                .WithMany(c => c.LoginRecords)
                .HasForeignKey(o => o.UserId);
            
            entity
                .HasMany(o => o.RefreshRecords);
        });

        modelBuilder.Entity<RefreshRecordEntity>(entity =>
        {
            entity
                .HasOne(o => o.LoginRecord)
                .WithMany(c => c.RefreshRecords)
                .HasForeignKey(o => o.LoginRecordId);
        });
    }
}