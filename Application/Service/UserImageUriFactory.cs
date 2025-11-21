using Application.Service.OAuth;
using Database.Entity;
using Microsoft.Extensions.Options;
using Presentation.Configuration.Options;
using Presentation.Service;

namespace Application.Service;

public class UserImageUriFactory(
    IOptionsSnapshot<AuthOptions> authOptions) : IUserImageUriFactory
{
    public Uri GetSmallImageUri(ImageEntity image)
    {
        return new OAuthUriBuilder(authOptions.Value.Self)
            .SetPath($"image/{image.Id}/small")
            .Build();
    }

    public Uri GetMediumImageUri(ImageEntity image)
    {
        return new OAuthUriBuilder(authOptions.Value.Self)
            .SetPath($"image/{image.Id}/medium")
            .Build();
    }

    public Uri GetLargeImageUri(ImageEntity image)
    {
        return new OAuthUriBuilder(authOptions.Value.Self)
            .SetPath($"image/{image.Id}/large")
            .Build();
    }
}