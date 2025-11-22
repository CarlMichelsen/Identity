namespace Presentation.Service.OAuth.JsonWebToken;

public interface ICookieApplier
{
    public void SetCookie(TokenType tokenType, string value);

    public void DeleteCookie(TokenType tokenType);
}