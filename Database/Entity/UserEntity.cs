using System.ComponentModel.DataAnnotations;
using Database.Entity.Converter;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class UserEntity : IEntity
{
    public required UserEntityId Id { get; init; } = null!;
    
    [MinLength(2)]
    [MaxLength(1028)]
    public required string AuthenticationProviderId { get; init; }
    
    [MinLength(2)]
    [MaxLength(1028)]
    public required string AuthenticationProvider { get; init; }
    
    [MinLength(2)]
    [MaxLength(256)]
    public required string Username { get; init; }
    
    [MinLength(2)]
    [MaxLength(256)]
    public required string Email { get; init; }
    
    public required Uri RawAvatarUrl { get; init; }

    public List<LoginEntity> Login { get; init; } = [];
    
    public List<RefreshEntity> Refresh { get; init; } = [];
    
    public List<AccessEntity> Access { get; init; } = [];
    
    public List<string> Roles { get; init; } = [];
    
    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<UserEntity>();
        
        entityBuilder.HasKey(e => e.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<UserEntity, UserEntityId>(x =>
                new UserEntityId(x, true));
        
        entityBuilder
            .Property(x => x.Roles)
            .HasConversion(RoleConverterFactory.CreateConverter())
            .Metadata
            .SetValueComparer(RoleConverterFactory.CreateComparer());
        
        entityBuilder
            .HasMany(x => x.Login)
            .WithOne(x => x.User);
        
        entityBuilder
            .HasMany(x => x.Refresh)
            .WithOne(x => x.User);
        
        entityBuilder
            .HasMany(x => x.Access)
            .WithOne(x => x.User);
    }
}