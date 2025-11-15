using Database.Entity;
using Presentation.Service.OAuth.Model.Token;

namespace Presentation.Service.OAuth.Login.Receive;

/// <summary>
/// Is responsible for creating a token-pair from a new login.
/// Is also responsible for handling properly completing the login in the database - persisting the tokens ect.
/// </summary>
public interface ITokenPersistenceService
{
    Task<TokenPair> CreateAndPersistTokenPair(
        LoginEntity loginEntity,
        OAuthProcessEntity oAuthProcessEntity);
}