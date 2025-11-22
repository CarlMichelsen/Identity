using Presentation.Dto;

namespace Presentation.Service.OAuth.Login;

public interface ILoginRedirectService
{
    Task<Uri> GetLoginRedirectUri(
        AuthenticationProvider provider,
        LoginQueryDto loginQueryDto);
}