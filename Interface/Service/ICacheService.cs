using Domain.Abstraction;

namespace Interface.Service;

public interface ICacheService
{
    Task<Result<T>> GetValue<T>(string key);
    
    Task<Result> SetValue<T>(string key, T value, TimeSpan ttl);
    
    Task<Result> RemoveValue(string key);
}