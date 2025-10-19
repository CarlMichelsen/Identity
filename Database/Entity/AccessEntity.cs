using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class AccessEntity : BaseConnectionMetadata, IEntity
{
    public required AccessEntityId Id { get; init; }
    
    [MinLength(16)]
    [MaxLength(16384)]
    public required string AccessToken { get; init; }
    
    public required RefreshEntityId RefreshId { get; init; }
    
    public RefreshEntity? Refresh { get; init; }
    
    public required LoginEntityId LoginId { get; init; }
    
    public LoginEntity? Login { get; init; }
    
    public required UserEntityId UserId { get; init; }
    
    public UserEntity? User { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<AccessEntity>();
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<AccessEntity, AccessEntityId>(x =>
                new AccessEntityId(x, true));
        
        entityBuilder
            .Property(x => x.RefreshId)
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
    }
}