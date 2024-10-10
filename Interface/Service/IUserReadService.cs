using Domain.Abstraction;
using Domain.Dto;
using Domain.User;

namespace Interface.Service;

public interface IUserReadService
{
    Result<ServiceResponse<AuthenticatedUser>> GetUser();
}