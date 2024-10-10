using Domain.Abstraction;
using Domain.Dto;
using Implementation.Map;
using Interface.Accessor;
using Interface.Repository;
using Interface.Service;

namespace Implementation.Service;

public class SessionService(
    IUserContextAccessor userContextAccessor,
    ISessionReadRepository sessionReadRepository,
    ISessionInvalidationRepository sessionInvalidationRepository) : ISessionService
{
    public async Task<Result<ServiceResponse<List<LoginDto>>>> InvalidateSessions(
        List<long> loginIds)
    {
        var userContextResult = userContextAccessor.GetUserContext();
        if (userContextResult.IsError)
        {
            return userContextResult.Error!;
        }

        var userContext = userContextResult.Unwrap();
        var invalidatedSessionsResult = await sessionInvalidationRepository
            .InvalidateSessions(userContext.User.Id, loginIds);
        if (invalidatedSessionsResult.IsError)
        {
            return invalidatedSessionsResult.Error!;
        }
        
        var dtoResult = LoginMapper.Map(invalidatedSessionsResult.Unwrap());
        if (dtoResult.IsError)
        {
            return dtoResult.Error!;
        }

        return new ServiceResponse<List<LoginDto>>(dtoResult.Unwrap());
    }

    public async Task<Result<ServiceResponse<List<LoginDto>>>> GetSessions()
    {
        var userContextResult = userContextAccessor.GetUserContext();
        if (userContextResult.IsError)
        {
            return userContextResult.Error!;
        }

        var userContext = userContextResult.Unwrap();
        var sessionsResult = await sessionReadRepository.GetSessions(userContext.User.Id);
        if (sessionsResult.IsError)
        {
            return sessionsResult.Error!;
        }
        
        var dtoResult = LoginMapper.Map(sessionsResult.Unwrap());
        if (dtoResult.IsError)
        {
            return dtoResult.Error!;
        }

        return new ServiceResponse<List<LoginDto>>(dtoResult.Unwrap());
    }
}