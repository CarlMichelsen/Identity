using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.OAuth;

public interface IOAuthLoginAccessControl
{
    Task<Result<LoginProcessContext>> GetProcessIdentifier(
        LoginRedirectInformation loginRedirectInformation);

    Task<Result<LoginProcessContext>> ValidateLoginProcess(
        Dictionary<string, string> queryParameters,
        Func<Dictionary<string, string>, Result<(Guid State, string Additional)>> stateCode,
        Func<string, Task<Result<IUserConvertible>>> getUser);
}