using Database.Entity;
using Database.Entity.Id;
using Presentation.Service.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Application.Service.Image;

/// <inheritdoc />
public class UserImageProcessor(
    TimeProvider timeProvider,
    HttpClient httpClient) : IUserImageProcessor
{
    private readonly Dictionary<ImageSize, int> sizes = new()
    {
        [ImageSize.Small] = 150,
        [ImageSize.Medium] = 400,
        [ImageSize.Large] = 800,
    };
    
    /// <inheritdoc />
    public async Task<ImageEntity> ProcessImage(
        Uri imageUri,
        CancellationToken cancellationToken = default)
    {
        if (!imageUri.IsAbsoluteUri)
        {
            throw new ImageException("Invalid image uri - must be an absolute URI.");
        }
        
        // Download Image once
        using var response = await httpClient.GetAsync(imageUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, cancellationToken);

        var result = new Dictionary<ImageSize, byte[]>();
        foreach (var (sizeName, dimension) in sizes)
        {
            using var resizedImage = image.Clone(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(dimension, dimension),
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center,
                    Sampler = KnownResamplers.Lanczos3
                });
            });
            
            await using var outputStream = new MemoryStream();
            await resizedImage.SaveAsJpegAsync(
                outputStream,
                new JpegEncoder
                {
                    Quality = 100,
                },
                cancellationToken);
            
            result.Add(sizeName, outputStream.ToArray());
        }

        var smallId = new ContentEntityId(Guid.CreateVersion7());
        var mediumId = new ContentEntityId(Guid.CreateVersion7());
        var largeId = new ContentEntityId(Guid.CreateVersion7());

        var now = timeProvider.GetUtcNow().UtcDateTime;
        return new ImageEntity
        {
            Id = new ImageEntityId(Guid.CreateVersion7()),
            Source = imageUri,
            SmallId = smallId,
            Small = new ContentEntity
            {
                Id = smallId,
                Data = result[ImageSize.Small],
                CreatedAt = now,
            },
            MediumId = mediumId,
            Medium = new ContentEntity
            {
                Id = mediumId,
                Data = result[ImageSize.Medium],
                CreatedAt = now,
            },
            LargeId = largeId,
            Large = new ContentEntity
            {
                Id = largeId,
                Data = result[ImageSize.Large],
                CreatedAt = now,
            },
        };
    }

    private enum ImageSize
    {
        Small,
        Medium,
        Large,
    }
}