using Microsoft.AspNetCore.Mvc;
using Presentation;
using Presentation.Dto;
using Presentation.Service.OAuth.JsonWebToken;
using Presentation.Service.OAuth.Login;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("Login/{provider:provider}")]
    public async Task<ActionResult> Login(
        [FromServices] ILoginRedirectService loginRedirectService,
        [FromRoute] string provider,
        [FromQuery] LoginQueryDto loginQueryDto)
    {
        if (!Enum.TryParse<AuthenticationProvider>(provider, ignoreCase: true, out var authenticationProvider))
        {
            return this.NotFound();
        }

        var uri = await loginRedirectService
            .GetLoginRedirectUri(authenticationProvider, loginQueryDto);
        return this.Redirect(uri.AbsoluteUri);
    }
    
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