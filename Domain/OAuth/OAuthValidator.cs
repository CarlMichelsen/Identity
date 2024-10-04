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
                $"Id is null or empty when mapping to {nameof(OAuthUser)}");
        }
        
        if (string.IsNullOrWhiteSpace(username))
        {
            return new ResultError(
                ResultErrorType.MapError,
                $"Username is null or empty when mapping to {nameof(OAuthUser)}");
        }
        
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            return new ResultError(
                ResultErrorType.MapError,
                $"Email is null or empty or invalid when mapping to {nameof(OAuthUser)}");
        }
        
        if (!Uri.TryCreate(avatarUrl, default(UriCreationOptions), out var uri))
        {
            return new ResultError(
                ResultErrorType.MapError,
                $"AvatarUrl is invalid when mapping to {nameof(OAuthUser)}");
        }

        return new OAuthUser
        {
            ProviderId = id,
            AuthenticationProvider = provider,
            Username = username,
            AvatarUrl = uri.AbsoluteUri,
            Email = email,
        };
    }
}