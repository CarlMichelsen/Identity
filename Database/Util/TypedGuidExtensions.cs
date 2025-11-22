using System.Linq.Expressions;
using Database.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Util;

public static class TypedGuidExtensions
{
    public static PropertyBuilder<TKey> RegisterTypedKeyConversion<TEntity, TKey>(
        this PropertyBuilder<TKey> propertyBuilder,
        Expression<Func<Guid, TKey>> convertToProviderExpression)
        where TEntity : IEntity
        where TKey : TypedGuid<TEntity>
    {
        propertyBuilder
            .HasConversion<Guid>(id => id.Value, convertToProviderExpression)
            .HasColumnType("uuid");
        
        return propertyBuilder;
    }
}