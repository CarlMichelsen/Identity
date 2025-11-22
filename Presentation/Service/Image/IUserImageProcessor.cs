using Database.Entity;
using Database.Entity.Id;

namespace Presentation.Service.Image;

public interface IUserImageProcessor
{
    /// <summary>
    /// Download an image, process the image and create an untracked ImageEntity to be saved in the database. 
    /// </summary>
    /// <param name="userId">Identifier for the user that the ImageEntity is linked to.</param>
    /// <param name="imageUri">Uri for an image resource.</param>
    /// <param name="cancellationToken">Cancel the operation.</param>
    /// <returns></returns>
    Task<ImageEntity> ProcessImage(
        UserEntityId userId,
        Uri imageUri,
        CancellationToken cancellationToken = default);
}