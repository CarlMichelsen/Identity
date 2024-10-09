using System.Text.Json;
using Domain.Abstraction;
using Domain.Configuration;
using Domain.Dto;
using Interface.Service;
using Microsoft.AspNetCore.Http;

namespace Implementation.Service;

public class UserReadService(
    IHttpContextAccessor httpContextAccessor) : IUserReadService
{
    private const string JsonUserClaimType = "user";
    
    public Result<ServiceResponse<AuthenticatedUser>> GetUser()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return new ResultError(
                ResultErrorType.MapError,
                "No httpContext found");
        }

        if (httpContextAccessor.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return new ServiceResponse<AuthenticatedUser>(ApplicationConstants.UnauthorizedErrorMessage);
        }
        
        var userClaim = httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == JsonUserClaimType)?.Value;
        if (string.IsNullOrWhiteSpace(userClaim))
        {
            return new ResultError(
                ResultErrorType.JsonParse,
                "\"{UserClaim}\" claim was null or whitespace in an authenticated jwt token");
        }
            
        AuthenticatedUser? user;
        try
        {
            user = JsonSerializer.Deserialize<AuthenticatedUser>(userClaim);
        }
        catch (JsonException e)
        {
            return new ResultError(
                ResultErrorType.JsonParse,
                userClaim,
                e);
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "Something really bad happened during json deserialization of AuthenticatedUser from claims",
                e);
        }

        if (user is null)
        {
            return new ResultError(
                ResultErrorType.JsonParse,
                "json deserialization of AuthenticatedUser resulted in a null value");
        }
        
        return new ServiceResponse<AuthenticatedUser>(user);
    }
}