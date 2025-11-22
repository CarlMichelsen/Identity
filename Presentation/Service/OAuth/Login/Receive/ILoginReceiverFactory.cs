namespace Presentation.Service.OAuth.Login.Receive;

public interface ILoginReceiverFactory
{
    ILoginReceiver Create(AuthenticationProvider provider);
}