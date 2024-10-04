using Domain.Abstraction;
using Domain.OAuth;

namespace Interface.Service;

public interface ILoginCookieWriterService
{
    Result WriteLoginCookies(LoginProcessContext processContext);
}