using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RandomController(
    ILogger<RandomController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> Get([FromQuery] int length = 16)
    {
        var task = Task.Delay(TimeSpan.FromMilliseconds(200));
        
        if (length > 65536)
        {
            return this.BadRequest();
        }
        
        logger.LogInformation(
            "Random string of length {Length} was generated for {Host}",
            length,
            this.HttpContext.Request.Host.Host);
        var randomString = GenerateRandomString(length);
        await task;
        return randomString;
    }
    
    private static string GenerateRandomString(int length)
    {
        // Base64 expands binary data by ~4/3
        var byteLength = (int)Math.Ceiling(length * 3 / 4.0);

        var bytes = RandomNumberGenerator.GetBytes(byteLength);
        var base64 = Convert.ToBase64String(bytes);
        
        var base64String = base64.Length > length ? base64[..length] : base64;
        return base64String
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}