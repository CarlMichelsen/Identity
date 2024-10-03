using Domain.Abstraction;
using Domain.OAuth.Development;

namespace Interface.Service;

public interface IDevelopmentUserService
{
    Task<Result<DevelopmentUserDto>> GetDevelopmentUserFromAccessToken(string accessToken);
}