using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Integration.Util;

public static class EfcoreExtensions
{
    public static IServiceCollection RemoveAllEfCoreServices(this IServiceCollection services)
    {
        var efCoreServiceTypes = new[]
        {
            // DbContext types
            typeof(DbContext),
            typeof(DbContextOptions),
            
            // Database provider services
            typeof(IRelationalConnection),
            typeof(Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator),
            typeof(Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator),
            typeof(Microsoft.EntityFrameworkCore.Storage.IRelationalConnection),
            typeof(Microsoft.EntityFrameworkCore.Storage.IRelationalCommandBuilderFactory),
            typeof(Microsoft.EntityFrameworkCore.Storage.IRelationalTypeMappingSource),
            
            // Query services
            typeof(IQueryCompiler),
            typeof(Microsoft.EntityFrameworkCore.Query.IQueryContextFactory),
            
            // Infrastructure services
            typeof(IDbContextFactory<>),
            typeof(IDbContextPool<>),
            typeof(Microsoft.EntityFrameworkCore.Infrastructure.IModelCacheKeyFactory),
            typeof(Microsoft.EntityFrameworkCore.Infrastructure.IModelCustomizer),
            typeof(Microsoft.EntityFrameworkCore.Infrastructure.IModelSource),
        };

        // Remove by exact type match
        foreach (var serviceType in efCoreServiceTypes)
        {
            var descriptors = services.Where(d => d.ServiceType == serviceType).ToList();
            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }
        }

        // Remove generic DbContextOptions<T>
        var genericOptionsDescriptors = services
            .Where(d => d.ServiceType.IsGenericType && 
                       (d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) ||
                        d.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextFactory<>) ||
                        d.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextPool<>)))
            .ToList();

        foreach (var descriptor in genericOptionsDescriptors)
        {
            services.Remove(descriptor);
        }

        // Remove all DbContext subclasses
        var dbContextDescriptors = services
            .Where(d => typeof(DbContext).IsAssignableFrom(d.ServiceType))
            .ToList();

        foreach (var descriptor in dbContextDescriptors)
        {
            services.Remove(descriptor);
        }

        return services;
    }
}