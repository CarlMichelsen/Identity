namespace Presentation.Service.OAuth.Model.Token;

public class TokenPair
{
    public required AccessToken AccessToken { get; init; }

    public required RefreshToken RefreshToken { get; init; }
}