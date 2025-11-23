namespace Presentation.Service.OAuth.LoginCallback;

public interface ILoginReceiverRedirectService
{
    Task<Uri> PerformLoginAndCreateRedirectUri(AuthenticationProvider provider, Dictionary<string, string> parameters);
}