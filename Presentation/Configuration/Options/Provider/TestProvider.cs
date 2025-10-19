using Database;

namespace Presentation.Configuration.Options.Provider;

public class TestProvider : BaseProvider
{
    public override AuthenticationProvider ProviderType { get; } = AuthenticationProvider.Test;
}