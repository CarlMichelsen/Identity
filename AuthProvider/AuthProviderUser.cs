using Presentation;

namespace AuthProvider;

public record AuthProviderUser(
    string AuthenticationProviderId,
    AuthenticationProvider AuthenticationProvider,
    string Username,
    string Email,
    Uri? AvatarUrl);