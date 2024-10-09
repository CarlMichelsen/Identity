using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface IUserRefreshHandler
{
    Task<IResult> Refresh();
    
    Task<IResult> Logout();
}