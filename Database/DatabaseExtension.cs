using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Entity;

public static class DatabaseExtension
{
    public static async Task EnsureDatabaseUpdated(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await dbContext.Database.MigrateAsync();
    }
}