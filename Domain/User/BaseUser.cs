namespace Domain.User;

public abstract class BaseUser
{
    public required Guid Id { get; init; }
    
    public required AuthenticationProvider Provider { get; init; }
    
    public required string Username { get; init; }
    
    public required string Email { get; init; }
    
    public required DateTime CreatedAt { get; init; }
}