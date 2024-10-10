using Domain.Abstraction;
using Domain.Configuration;
using Domain.Dto;
using Domain.User;
using Interface.Accessor;
using Interface.Service;

namespace Implementation.Service;

public class UserReadService(
    IUserContextAccessor userContextAccessor) : IUserReadService
{
    public Result<ServiceResponse<AuthenticatedUser>> GetUser()
    {
        var userContextResult = userContextAccessor.GetUserContext();
        if (userContextResult.IsError)
        {
            return new ServiceResponse<AuthenticatedUser>(
                ApplicationConstants.UnauthorizedErrorMessage);
        }
        
        return new ServiceResponse<AuthenticatedUser>(userContextResult.Unwrap().User);
    }
}