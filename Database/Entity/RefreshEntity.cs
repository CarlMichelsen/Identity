using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class RefreshEntity : BaseConnectionMetadata, IEntity
{
    public required RefreshEntityId Id { get; init; }
    
    [MinLength(16)]
    [MaxLength(16384)]
    public required string HashedRefreshToken { get; init; }
    
    public required LoginEntityId LoginId { get; init; }
    
    public LoginEntity? Login { get; init; }
    
    public required UserEntityId UserId { get; init; }
    
    // ReSharper disable once EntityFramework.ModelValidation.CircularDependency
    public UserEntity? User { get; init; }

    public List<AccessEntity> Access { get; init; } = [];
    
    public required DateTime ValidUntil { get; set; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<RefreshEntity>();
        
        entityBuilder.HasKey(e => e.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<RefreshEntity, RefreshEntityId>(x =>
                new RefreshEntityId(x, true));
        
        entityBuilder
            .Property(x => x.LoginId)
            .RegisterTypedKeyConversion<LoginEntity, LoginEntityId>(x =>
                new LoginEntityId(x, true));
        
        entityBuilder
            .Property(x => x.UserId)
            .RegisterTypedKeyConversion<UserEntity, UserEntityId>(x =>
                new UserEntityId(x, true));
        
        entityBuilder
            .HasOne(x => x.User)
            .WithMany(x => x.Refresh)
            .HasForeignKey(x => x.UserId);
        
        entityBuilder
            .HasMany(x => x.Access)
            .WithOne(x => x.Refresh)
            .HasForeignKey(x => x.RefreshId);
    }
}