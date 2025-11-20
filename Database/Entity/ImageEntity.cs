using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class ImageEntity : IEntity
{
    public required ImageEntityId Id { get; init; }
    
    public required Uri Source { get; init; }
    
    // User
    public required UserEntityId UserId { get; init; }
    public UserEntity? User { get; init; }
    
    // Small
    public required ContentEntityId SmallId { get; init; }
    public ContentEntity? Small { get; init; }
    
    // Medium
    public required ContentEntityId MediumId { get; init; }
    public ContentEntity? Medium { get; init; }
    
    // Large
    public required ContentEntityId LargeId { get; init; }
    public ContentEntity? Large { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<ImageEntity>();
        
        entityBuilder.HasKey(e => e.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<ImageEntity, ImageEntityId>(x =>
                new ImageEntityId(x, true));
        
        // Small
        entityBuilder
            .Property(x => x.UserId)
            .RegisterTypedKeyConversion<UserEntity, UserEntityId>(x =>
                new UserEntityId(x, true));
        entityBuilder
            .HasOne(x => x.User)
            .WithOne(x => x.Image)
            .HasForeignKey<ImageEntity>(x => x.UserId);
        
        // Small
        entityBuilder
            .Property(x => x.SmallId)
            .RegisterTypedKeyConversion<ContentEntity, ContentEntityId>(x =>
                new ContentEntityId(x, true));
        entityBuilder
            .HasOne(x => x.Small)
            .WithMany()
            .HasForeignKey(x => x.SmallId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Medium
        entityBuilder
            .Property(x => x.MediumId)
            .RegisterTypedKeyConversion<ContentEntity, ContentEntityId>(x =>
                new ContentEntityId(x, true));
        entityBuilder
            .HasOne(x => x.Medium)
            .WithMany()
            .HasForeignKey(x => x.MediumId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Large
        entityBuilder
            .Property(x => x.LargeId)
            .RegisterTypedKeyConversion<ContentEntity, ContentEntityId>(x =>
                new ContentEntityId(x, true));
        entityBuilder
            .HasOne(x => x.Large)
            .WithMany()
            .HasForeignKey(x => x.LargeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}