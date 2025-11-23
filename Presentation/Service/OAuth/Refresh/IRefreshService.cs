namespace Presentation.Service.OAuth.Refresh;

public interface IRefreshService
{
    Task<bool> HandleRefresh();
}