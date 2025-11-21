using Database.Entity;

namespace Presentation.Service;

public interface IUserImageUriFactory
{
    Uri GetSmallImageUri(ImageEntity image);
    
    Uri GetMediumImageUri(ImageEntity image);
    
    Uri GetLargeImageUri(ImageEntity image);
}