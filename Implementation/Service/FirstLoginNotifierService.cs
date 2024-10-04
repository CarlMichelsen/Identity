using Interface.Service;

namespace Implementation.Service;

public class FirstLoginNotifierService : IFirstLoginNotifierService
{
    // TODO: actually notify
    public Task FirstLogin(long userId)
    {
        return Task.CompletedTask;
    }
}