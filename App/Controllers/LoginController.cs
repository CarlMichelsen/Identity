using Microsoft.AspNetCore.Mvc;
using Presentation.Configuration.Options.Provider;
using Presentation.Dto;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LoginController(
    ILogger<LoginController> logger) : ControllerBase
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

        logger.LogInformation(
            "{Provider}\nSuccess: {SuccessRedirectUri}\nError: {ErrorRedirectUri}",
            authenticationProvider,
            loginQueryDto.SuccessRedirectUri,
            loginQueryDto.ErrorRedirectUri);

        await Task.Delay(TimeSpan.FromMilliseconds(1));
        return this.Ok(authenticationProvider);
    }
}