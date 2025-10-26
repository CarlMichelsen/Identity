using Microsoft.AspNetCore.Mvc;
using Presentation;
using Presentation.Dto;
using Presentation.Service.OAuth.Login;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LoginController(
    ILoginRedirectService loginRedirectService) : ControllerBase
{
    [HttpGet("{provider:provider}")]
    public async Task<ActionResult> Login(
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
}