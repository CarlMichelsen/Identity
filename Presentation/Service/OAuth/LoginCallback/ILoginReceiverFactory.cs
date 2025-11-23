namespace Presentation.Service.OAuth.LoginCallback;

public interface ILoginReceiverFactory
{
    ILoginReceiver Create(AuthenticationProvider provider);
}