using Domain.Abstraction;
using Domain.Dto;

namespace Interface.Service;

public interface IUserReadService
{
    Result<ServiceResponse<AuthenticatedUser>> GetUser();
}