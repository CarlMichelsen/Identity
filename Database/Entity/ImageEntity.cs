using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class ImageEntity : IEntity
{
    public required ImageEntityId Id { get; init; }
    
    public required Uri Source { get; init; }
    
    // Small
    public required ContentEntityId SmallId { get; init; }
    public required ContentEntity Small { get; init; }
    
    // Medium
    public required ContentEntityId MediumId { get; init; }
    public required ContentEntity Medium { get; init; }
    
    // Large
    public required ContentEntityId LargeId { get; init; }
    public required ContentEntity Large { get; init; }
    
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