using App.ModelBinder;
using Application.Configuration.Options.Provider;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LoginController(
    ILogger<LoginController> logger) : ControllerBase
{
    [HttpGet("{provider}")]
    public async Task<ActionResult> Login(
        [FromRoute, ModelBinder(typeof(CaseInsensitiveEnumBinder<AuthenticationProvider>))] AuthenticationProvider provider)
    {
        logger.LogInformation("{Provider}", provider);
        await Task.Delay(TimeSpan.FromSeconds(1));
        throw  new NotImplementedException();
    }
}