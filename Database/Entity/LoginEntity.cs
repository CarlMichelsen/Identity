using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class LoginEntity : IEntity
{
    public required LoginEntityId Id { get; init; }
    
    public required UserEntityId UserId { get; init; }
    
    public UserEntity? User { get; init; }
    
    public required OAuthProcessEntityId OAuthProcessId { get; init; }
    
    public OAuthProcessEntity? OAuthProcess { get; init; }

    public List<RefreshEntity> Refresh { get; init; } = [];
    
    public List<AccessEntity> Access { get; init; } = [];

    public bool FirstLogin { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<LoginEntity>();
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<LoginEntity, LoginEntityId>(x =>
                new LoginEntityId(x, true));
        
        entityBuilder
            .Property(x => x.UserId)
            .RegisterTypedKeyConversion<UserEntity, UserEntityId>(x =>
                new UserEntityId(x, true));
        
        entityBuilder
            .HasOne(x => x.User)
            .WithMany(x => x.Login)
            .HasForeignKey(x => x.UserId);
        
        entityBuilder
            .Property(x => x.OAuthProcessId)
            .RegisterTypedKeyConversion<OAuthProcessEntity, OAuthProcessEntityId>(x =>
                new OAuthProcessEntityId(x, true));

        entityBuilder
            .HasOne(x => x.OAuthProcess)
            .WithOne(x => x.Login)
            .HasForeignKey<LoginEntity>(x => x.OAuthProcessId);
        
        entityBuilder
            .HasMany(x => x.Refresh)
            .WithOne(x => x.Login)
            .HasForeignKey(x => x.LoginId);
        
        entityBuilder
            .HasMany(x => x.Access)
            .WithOne(x => x.Login)
            .HasForeignKey(x => x.LoginId);
    }
}