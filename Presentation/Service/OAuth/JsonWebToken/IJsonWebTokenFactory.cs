using Database.Entity;
using Database.Entity.Id;
using Presentation.Service.OAuth.Model.Token;

namespace Presentation.Service.OAuth.JsonWebToken;

public interface IJsonWebTokenFactory
{
    TokenPair CreateTokenPairFromNewLoginEntity(
        LoginEntity loginEntity,
        RefreshEntityId refreshEntityId);
}