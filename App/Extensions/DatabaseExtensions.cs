using System.Reflection;
using Database;
using Microsoft.EntityFrameworkCore;

namespace App.Extensions;

public static class DatabaseExtensions
{
    public static async Task EnsureLatestDatabaseMigration(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static void AddDatabase<TContext>(this WebApplicationBuilder builder)
        where TContext : DbContext
    {
        if (builder.Configuration.GetValue<string>("IntegrationTest") == "true")
        {
            return;
        }
        
        builder.Services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b =>
                    {
                        var assemblyName = Assembly
                            .GetExecutingAssembly()
                            .GetName()
                            .Name ?? throw new NullReferenceException("Unable to get assembly name");
                        b.MigrationsAssembly(assemblyName);
                        b.MigrationsHistoryTable("__EFMigrationsHistory", DatabaseContext.SchemaName);
                    })
                .UseSnakeCaseNamingConvention();
            
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }
}