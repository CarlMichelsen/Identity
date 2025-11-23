namespace Presentation.Service.OAuth.Refresh;

public interface ICookieApplier
{
    public void SetCookie(TokenType tokenType, string value);

    public void DeleteCookie(TokenType tokenType);
}