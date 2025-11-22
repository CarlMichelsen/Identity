namespace Presentation.Service.OAuth.Model;

public record AuthenticatedUser(
    string AuthenticationProviderId,
    AuthenticationProvider AuthenticationProvider,
    string Username,
    string Email,
    Uri AvatarUrl);