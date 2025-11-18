using Database.Entity.Id;
using Microsoft.Extensions.Logging;

namespace Application;

public static partial class Logs
{
    [LoggerMessage(LogLevel.Information, "A refresh token was minted by user <{userId}> with id <{refreshEntityId}>")]
    public static partial void LogARefreshTokenWasMintedByUserUseridWithIdRefreshEntityId(
        this ILogger logger,
        UserEntityId userId,
        RefreshEntityId refreshEntityId);

    [LoggerMessage(LogLevel.Information, "An access token was minted by user <{userId}> with id <{accessEntityId}>")]
    public static partial void LogAnAccessTokenWasMintedByUserUseridWithIdAccessEntityId(
        this ILogger logger,
        UserEntityId userId,
        AccessEntityId accessEntityId);
}