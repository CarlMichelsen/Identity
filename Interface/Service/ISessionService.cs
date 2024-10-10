using Domain.Abstraction;
using Domain.Dto;

namespace Interface.Service;

public interface ISessionService
{
    Task<Result<ServiceResponse<List<LoginDto>>>> InvalidateSessions(List<long> loginIds);
    
    Task<Result<ServiceResponse<List<LoginDto>>>> GetSessions();
}