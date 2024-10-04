namespace Interface.Service;

public interface IFirstLoginNotifierService
{
    Task FirstLogin(long userId);
}