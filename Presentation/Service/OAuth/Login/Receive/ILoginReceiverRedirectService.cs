namespace Presentation.Service.OAuth.Login.Receive;

public interface ILoginReceiverRedirectService
{
    Task<Uri> PerformLoginAndCreateRedirectUri(AuthenticationProvider provider, Dictionary<string, string> parameters);
}