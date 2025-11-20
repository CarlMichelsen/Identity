using Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class DatabaseContext(
    DbContextOptions<DatabaseContext> options)
    : DbContext(options)
{
    public const string SchemaName = "identity";
    
    public DbSet<ContentEntity> Content => Set<ContentEntity>();

    public DbSet<OAuthProcessEntity> OAuthProcess => Set<OAuthProcessEntity>();

    public DbSet<UserEntity> User => Set<UserEntity>();
    
    public DbSet<ImageEntity> Image => Set<ImageEntity>();
    
    public DbSet<LoginEntity> Login => Set<LoginEntity>();
    
    public DbSet<RefreshEntity> Refresh => Set<RefreshEntity>();
    
    public DbSet<AccessEntity> Access => Set<AccessEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        
        ContentEntity.Configure(modelBuilder);
        OAuthProcessEntity.Configure(modelBuilder);

        UserEntity.Configure(modelBuilder);
        ImageEntity.Configure(modelBuilder);
        LoginEntity.Configure(modelBuilder);
        RefreshEntity.Configure(modelBuilder);
        AccessEntity.Configure(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
}