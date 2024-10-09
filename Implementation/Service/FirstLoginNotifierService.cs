using Database;
using Interface.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Implementation.Service;

public class FirstLoginNotifierService(
    ILogger<FirstLoginNotifierService> logger,
    ApplicationContext applicationContext) : IFirstLoginNotifierService
{
    public async Task FirstLogin(long loginId)
    {
        var login = await applicationContext.LoginRecord
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == loginId);
        if (login is null)
        {
            return;
        }
        
        logger.LogInformation(
            "First login for \"{Username}\" with |{ProviderName}| from \"{Ip}\"",
            login.User.Username,
            Enum.GetName(login.User.AuthenticationProvider),
            login.Ip);
    }
}