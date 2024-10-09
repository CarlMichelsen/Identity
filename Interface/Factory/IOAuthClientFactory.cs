using Database.Entity;
using Domain.Abstraction;
using Interface.OAuth;

namespace Interface.Factory;

public interface IOAuthClientFactory
{
    Result<IOAuthClient> Create(OAuthProvider provider);
}