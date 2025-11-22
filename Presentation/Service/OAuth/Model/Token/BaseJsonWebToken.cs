namespace Presentation.Service.OAuth.Model.Token;

public abstract class BaseJsonWebToken
{
    public required string Token { get; init; }
}