using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class OAuthProcessEntity : IEntity
{
    public required LoginProcessEntityId Id { get; init; }
    
    [MinLength(12)]
    [MaxLength(1028)]
    public required string State { get; init; }
    
    public required Uri LoginRedirectUri { get; init; }
    
    public required Uri SuccessRedirectUri { get; init; }
    
    public required Uri ErrorRedirectUri { get; init; }
    
    public UserEntityId? UserId { get; set; }
    
    public UserEntity? User { get; set; }
    
    public LoginEntityId? LoginId { get; set; }
    
    public LoginEntity? Login { get; set; }
    
    [MinLength(1)]
    [MaxLength(1028)]
    public string? Error { get; set; }
    
    public DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<OAuthProcessEntity>();
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<OAuthProcessEntity, LoginProcessEntityId>(x =>
                new LoginProcessEntityId(x, true));
        
        entityBuilder
            .Property(x => x.UserId)!
            .RegisterTypedKeyConversion<UserEntity, UserEntityId>(x =>
                new UserEntityId(x, true));
        
        entityBuilder
            .HasOne<UserEntity>(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
        
        entityBuilder
            .Property(x => x.LoginId)!
            .RegisterTypedKeyConversion<LoginEntity, LoginEntityId>(x =>
                new LoginEntityId(x, true));
        
        entityBuilder
            .HasOne<LoginEntity>(x => x.Login)
            .WithOne(x => x.LoginProcess)
            .HasForeignKey<OAuthProcessEntity>(x => x.LoginId);
    }
}