using Domain.Abstraction;
using Domain.OAuth;
using Interface.OAuth;

namespace Interface.Factory;

public interface IOAuthClientFactory
{
    Result<IOAuthClient> Create(OAuthProvider provider);
}