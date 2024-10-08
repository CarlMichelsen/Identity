using Domain.OAuth.Development;

namespace Implementation.Service;

public static class DevelopmentUsers
{
    public static readonly Dictionary<long, DevelopmentUserDto> Users = new()
    {
        { 1, CreateUser(1, "Lars Vegas") },
        { 2, CreateUser(2, "Nicki Mirage") },
        { 3, CreateUser(3, "Bruce Leeglad") },
        { 4, CreateUser(4, "Karen Management") },
        { 5, CreateUser(5, "Spiderman") },
    };

    private static DevelopmentUserDto CreateUser(long id, string username)
    {
        var safeUsername = username.Trim();
        var noSpacesUsername = safeUsername.Replace(' ', '-').ToLower();
        return new DevelopmentUserDto
        {
            Id = id.ToString(),
            Username = safeUsername,
            AvatarUrl = $"https://api.dicebear.com/9.x/adventurer/svg?seed={noSpacesUsername}",
            Email = $"{noSpacesUsername}.{id}@testUser.com",
        };
    }
}