using Database;
using Database.Entity.Id;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace App.Controllers;

[ApiController]
[Route("[controller]/{imageId:guid}")]
[EnableRateLimiting("images")]
public class ImageController(
    DatabaseContext databaseContext) : ControllerBase
{
    private const string ContentType = "image/jpeg";
    private const int CacheDurationSeconds = 2628000; // 1 month
    
    [HttpGet("small")]
    public async Task<IActionResult> Small([FromRoute] Guid imageId)
    {
        var imageEntityId = new ImageEntityId(imageId);
        var imageEntity = await databaseContext.Image
            .Include(i => i.Small)
            .FirstOrDefaultAsync(i => i.Id == imageEntityId);
        
        this.Response.Headers.CacheControl = $"public, max-age={CacheDurationSeconds}, immutable";

        return imageEntity?.Small is null
            ? this.NotFound()
            : this.File(
                imageEntity.Small.Data,
                ContentType,
                lastModified: imageEntity.Small.CreatedAt,
                entityTag: new EntityTagHeaderValue($"\"{imageId}-small\""));
    }
    
    [HttpGet("medium")]
    public async Task<IActionResult> Medium([FromRoute] Guid imageId)
    {
        var imageEntityId = new ImageEntityId(imageId);
        var imageEntity = await databaseContext.Image
            .Include(i => i.Medium)
            .FirstOrDefaultAsync(i => i.Id == imageEntityId);
        
        this.Response.Headers.CacheControl = $"public, max-age={CacheDurationSeconds}, immutable";
        
        return imageEntity?.Medium is null
            ? this.NotFound()
            : this.File(
                imageEntity.Medium.Data,
                ContentType,
                lastModified: imageEntity.Medium.CreatedAt,
                entityTag: new EntityTagHeaderValue($"\"{imageId}-medium\""));
    }
    
    [HttpGet("large")]
    public async Task<IActionResult> Large([FromRoute] Guid imageId)
    {
        var imageEntityId = new ImageEntityId(imageId);
        var imageEntity = await databaseContext.Image
            .Include(i => i.Large)
            .FirstOrDefaultAsync(i => i.Id == imageEntityId);
        
        this.Response.Headers.CacheControl = $"public, max-age={CacheDurationSeconds}, immutable";

        return imageEntity?.Large is null
            ? this.NotFound()
            : this.File(
                imageEntity.Large.Data,
                ContentType,
                lastModified: imageEntity.Large.CreatedAt,
                entityTag: new EntityTagHeaderValue($"\"{imageId}-large\""));
    }
}