namespace Presentation.Service.OAuth.JsonWebToken;

public interface IRefreshService
{
    Task<bool> HandleRefresh();
}