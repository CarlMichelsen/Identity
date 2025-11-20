using Database;
using Database.Entity;
using Microsoft.EntityFrameworkCore;
using Presentation.Service.Image;

namespace App.HostedServices;

public class ProfileImageProcessor(
    ILogger<ProfileImageProcessor> logger,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var imageProcessor = scope.ServiceProvider.GetRequiredService<IUserImageProcessor>();
            var user = await GetUserToBeProcessed(dbContext, stoppingToken);
            
            try
            {
                if (user is null)
                {
                    // All users have an image - hibernate a bit
                    var timespan = TimeSpan.FromHours(2);
                    logger.LogInformation("All users have an image - hibernating a for {TimeSpan}", timespan);
                    await Task.Delay(timespan, stoppingToken);
                    continue;
                }

                var imageEntity = await imageProcessor
                    .ProcessImage(user.RawAvatarUrl, stoppingToken);

                user.ImageId = imageEntity.Id;
                user.Image = imageEntity;
                logger.LogInformation("User {UserId} has been updated", user.Id);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning(
                    "{Processor} was cancelled",
                    nameof(ProfileImageProcessor));
            }
            catch (Exception e)
            {
                var waitTime = TimeSpan.FromMinutes(15);
                logger.LogError(
                    e,
                    "{Processor} failed - further image processing will be delayed by {TimeSpan}",
                    nameof(ProfileImageProcessor),
                    waitTime);
                await Task.Delay(waitTime, stoppingToken);
            }
            finally
            {
                if (user is not null)
                {
                    user.ImageProcessingAttempts++;
                }
                
                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }

    private static Task<UserEntity?> GetUserToBeProcessed(
        DatabaseContext dbContext,
        CancellationToken stoppingToken)
    {
        return dbContext
            .User
            .OrderByDescending(u => u.CreatedAt)
            .ThenBy(u => u.ImageProcessingAttempts)
            .Where(u => u.ImageProcessingAttempts < 5 && u.ImageId == null)
            .FirstOrDefaultAsync(stoppingToken);
    }
}
