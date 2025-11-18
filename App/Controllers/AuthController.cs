using Microsoft.AspNetCore.Mvc;
using Presentation.Service.OAuth.JsonWebToken;
using Presentation.Service.OAuth.Login;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("Refresh")]
    public async Task<ActionResult> Refresh([FromServices] IRefreshService refreshService)
    {
        return await refreshService.HandleRefresh()
            ? this.Ok()
            : this.Unauthorized();
    }
    
    [HttpDelete("Logout")]
    public async Task<ActionResult> Logout([FromServices] ILogoutService logoutService)
    {
        return await logoutService.HandleLogout()
            ? this.Ok()
            : this.Unauthorized();
    }
}