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
            var waitTime = TimeSpan.FromMinutes(1);
            
            UserEntity? user = null;
            try
            {
                user = await GetUserToBeProcessed(dbContext, stoppingToken);
                if (user is null)
                {
                    // All users have an image - hibernate for a bit
                    waitTime = TimeSpan.FromHours(1);
                    logger.LogInformation(
                        "All users have an image - hibernating a for {TimeSpan}",
                        waitTime);
                    throw new SkipException();
                }

                var imageEntity = await imageProcessor
                    .ProcessImage(user.Id, user.RawAvatarUrl, stoppingToken);

                user.ImageId = imageEntity.Id;
                user.Image = imageEntity;
                logger.LogInformation(
                    "User {UserId} has had their images updated",
                    user.Id);
            }
            catch (SkipException)
            {
                /* Do nothing here - just run delay in finally block */
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning(
                    "{Processor} was cancelled",
                    nameof(ProfileImageProcessor));
            }
            catch (Exception e)
            {
                waitTime = TimeSpan.FromMinutes(15);
                logger.LogError(
                    e,
                    "{Processor} failed - further image processing will be delayed by {TimeSpan}",
                    nameof(ProfileImageProcessor),
                    waitTime);
            }
            finally
            {
                if (user is not null)
                {
                    user.ImageProcessingAttempts++;
                }
                
                await dbContext.SaveChangesAsync(stoppingToken);
                await Task.Delay(waitTime, stoppingToken);
            }
        }
    }

    private static Task<UserEntity?> GetUserToBeProcessed(
        DatabaseContext dbContext,
        CancellationToken stoppingToken)
    {
        return dbContext
            .User
            .OrderBy(u => u.CreatedAt)
            .ThenBy(u => u.ImageProcessingAttempts)
            .Where(u => u.ImageProcessingAttempts < 5 && u.ImageId == null)
            .FirstOrDefaultAsync(stoppingToken);
    }

    private class SkipException : Exception;
}
