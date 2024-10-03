using Domain.Abstraction;
using Domain.OAuth.Development;
using Interface.Service;

namespace Implementation.Service;

public class DevelopmentUserService(
    ICacheService cacheService) : IDevelopmentUserService
{
    public async Task<Result<DevelopmentUserDto>> GetDevelopmentUserFromAccessToken(string accessToken)
    {
        var key = GenerateKeyFromAccessToken(accessToken);
        var userResult = await cacheService.GetValue<DevelopmentUserDto>(key);
        if (userResult.IsError)
        {
            return userResult.Error!;
        }
        
        var accessTokenInvalidatedResult = await cacheService.RemoveValue(accessToken);
        if (accessTokenInvalidatedResult.IsError)
        {
            return accessTokenInvalidatedResult.Error!;
        }

        return userResult.Unwrap();
    }

    private static string GenerateKeyFromAccessToken(string accessToken) => $"development-user-service-{accessToken}";
}