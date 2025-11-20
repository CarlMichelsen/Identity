using Database.Entity;

namespace Presentation.Service.Image;

public interface IUserImageProcessor
{
    /// <summary>
    /// Download an image, process the image and create an untracked ImageEntity to be saved in the database. 
    /// </summary>
    /// <param name="imageUri">Uri for an image resource.</param>
    /// <param name="cancellationToken">Cancel the operation.</param>
    /// <returns></returns>
    Task<ImageEntity> ProcessImage(
        Uri imageUri,
        CancellationToken cancellationToken = default);
}