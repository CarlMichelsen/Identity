namespace Presentation.Service.OAuth.Login;

public interface ILogoutService
{
    Task<bool> HandleLogout();
}