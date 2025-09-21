namespace Implementation.Configuration;

public sealed record OAuthProviderSection(
    string ClientId,
    string ClientSecret,
    string OAuthEndpoint,
    string OAuthReturnEndpoint);