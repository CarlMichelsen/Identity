using Microsoft.AspNetCore.Mvc;
using Presentation;
using Presentation.Service.OAuth.Login.Receive;

namespace App.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class OAuthController(
    ILoginReceiverRedirectService receiverFactory) : ControllerBase
{
    [HttpGet("Authorize/{provider:provider}")]
    public async Task<ActionResult> Authorize(
        [FromRoute] string provider,
        [FromQuery] Dictionary<string, string> parameters)
    {
        if (!Enum.TryParse<AuthenticationProvider>(provider, ignoreCase: true, out var authenticationProvider))
        {
            return this.NotFound();
        }

        var uri = await receiverFactory
            .PerformLoginAndCreateRedirectUri(authenticationProvider, parameters);
        
        return Redirect(uri.AbsoluteUri);
    }
}