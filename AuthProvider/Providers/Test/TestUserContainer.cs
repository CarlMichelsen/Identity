namespace AuthProvider.Providers.Test;

public static class TestUserContainer
{
    public static TestUser Steve { get; } = new TestUser
    {
        AuthenticationProviderId = nameof(Steve),
        Username = nameof(Steve),
        Email = $"{nameof(Steve)}@test-mail.com",
        AvatarUrl = GetAvatarUri(nameof(Steve))
    };
    
    public static TestUser Nicki { get; } = new TestUser
    {
        AuthenticationProviderId = nameof(Nicki),
        Username = nameof(Nicki),
        Email = $"{nameof(Nicki)}@test-mail.com",
        AvatarUrl = GetAvatarUri(nameof(Nicki))
    };
    
    public static TestUser Irene { get; } = new TestUser
    {
        AuthenticationProviderId = nameof(Irene),
        Username = nameof(Irene),
        Email = $"{nameof(Irene)}@test-mail.com",
        AvatarUrl = GetAvatarUri(nameof(Irene))
    };
    
    public static IReadOnlyDictionary<string, TestUser> GetUsers { get; } = new Dictionary<string, TestUser>
    {
        [Steve.AuthenticationProviderId] = Steve,
        [Nicki.AuthenticationProviderId] = Nicki,
        [Irene.AuthenticationProviderId] = Irene,
    }.AsReadOnly();

    private static Uri GetAvatarUri(string authenticationProviderId)
    {
        return new Uri($"https://api.dicebear.com/9.x/initials/png?seed={authenticationProviderId}");
    }
}