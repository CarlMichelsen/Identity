using Domain.Abstraction;
using Domain.Dto;

namespace Interface.Service;

public interface IUserLogoutService
{
    Task<Result<ServiceResponse>> Logout();
}