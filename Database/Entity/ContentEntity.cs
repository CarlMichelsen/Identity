using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

/// <summary>
/// This is a workaround for storing large amounts of data.
/// Sql databases are not meant for this so it is important that it remains possible to replace this entity with a more suitable solution.
/// </summary>
public class ContentEntity : IEntity
{
    public required ContentEntityId Id { get; init; }
    
    public required byte[] Data { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder.Entity<ContentEntity>();
        
        entityBuilder.HasKey(e => e.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<ContentEntity, ContentEntityId>(x =>
                new ContentEntityId(x, true));
    }
}