using Database.Entity;
using Database.Entity.Id;
using Presentation.Service.OAuth.Model.Token;

namespace Presentation.Service.OAuth.Refresh;

public interface IJsonWebTokenFactory
{
    TokenPair CreateTokenPairFromNewLoginEntity(
        LoginEntity loginEntity,
        RefreshEntityId refreshEntityId);
}