using Domain.Abstraction;

namespace Domain.OAuth;

public static class OAuthValidator
{
    public static Result<OAuthUser> Validate(
        string id,
        OAuthProvider provider,
        string username,
        string email,
        string avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "Id is null or empty when mapping IUserConvertible");
        }
        
        var authMethodResult = provider.MapToProviderString();
        if (authMethodResult.IsError)
        {
            return authMethodResult.Error!;
        }
        
        if (string.IsNullOrWhiteSpace(username))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "Username is null or empty when mapping IUserConvertible");
        }
        
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "Email is null or empty or invalid when mapping IUserConvertible");
        }
        
        if (!Uri.TryCreate(avatarUrl, default(UriCreationOptions), out var uri))
        {
            return new ResultError(
                ResultErrorType.MapError,
                "AvatarUrl is invalid when mapping IUserConvertible");
        }

        return new OAuthUser
        {
            Id = id,
            AuthenticationProvider = authMethodResult.Unwrap(),
            Username = username,
            AvatarUrl = avatarUrl,
            Email = email,
        };
    }
}