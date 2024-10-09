using Domain.Abstraction;
using Domain.Dto;
using Interface.Repository;
using Interface.Service;

namespace Implementation.Service;

public class UserLogoutService(
    IUserLogoutRepository userLogoutRepository) : IUserLogoutService
{
    public async Task<Result<ServiceResponse>> Logout()
    {
        throw new NotImplementedException();
    }
}