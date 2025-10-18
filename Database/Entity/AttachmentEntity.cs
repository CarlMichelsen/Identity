using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class AttachmentEntity : IEntity
{
    public required AttachmentEntityId Id { get; init; }
    
    public required IdentityEntityId IdentityId { get; init; }

    public required IdentityEntity? Identity { get; init; }
    
    public required ContentEntityId ContentId { get; init; }
    
    public ContentEntity? Content { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<AttachmentEntity>();
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<AttachmentEntity, AttachmentEntityId>(x =>
                new AttachmentEntityId(x, true));
        
        entityBuilder
            .Property(x => x.IdentityId)
            .RegisterTypedKeyConversion<IdentityEntity, IdentityEntityId>(x =>
                new IdentityEntityId(x, true));

        entityBuilder
            .HasOne(x => x.Identity)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.IdentityId);

        entityBuilder
            .HasOne(x => x.Content)
            .WithMany()
            .HasForeignKey(x => x.ContentId)
            .IsRequired();
    }
}