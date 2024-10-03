using System.Text.Json;
using Domain.Abstraction;
using Interface.Service;
using Microsoft.Extensions.Caching.Memory;

namespace Implementation.Service;

public class CacheService(
    IMemoryCache memoryCache) : ICacheService
{
    public Task<Result<T>> GetValue<T>(string key)
    {
        string? value = default;
        try
        {
            if (memoryCache.TryGetValue(key, out value))
            {
                var deserialized = JsonSerializer.Deserialize<T>(value!);
                if (deserialized is null)
                {
                    return Task.FromResult<Result<T>>(new ResultError(
                        ResultErrorType.NotFound,
                        $"Found null {nameof(T)} at \"{key}\""));
                }
                
                return Task.FromResult<Result<T>>(deserialized);
            }
            else
            {
                return Task.FromResult<Result<T>>(new ResultError(
                    ResultErrorType.NotFound,
                    $"Did not find {nameof(T)} at \"{key}\""));
            }
        }
        catch (JsonException e)
        {
            return Task.FromResult<Result<T>>(new ResultError(
                ResultErrorType.JsonParse,
                value ?? $"Json parse error for {nameof(T)} at \"{key}\" but there is no json string",
                e));
        }
        catch (Exception e)
        {
            return Task.FromResult<Result<T>>(new ResultError(
                ResultErrorType.Exception,
                $"Exception thrown when attempting to store {nameof(T)}",
                e));
        }
    }

    public Task<Result> SetValue<T>(string key, T value, TimeSpan ttl)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(value);
            memoryCache.Set(key, jsonString, ttl);
            return Task.FromResult(new Result());
        }
        catch (JsonException e)
        {
            return Task.FromResult<Result>(new ResultError(
                ResultErrorType.JsonParse,
                $"Json exception thrown when attempting to serialize {nameof(T)}",
                e));
        }
        catch (Exception e)
        {
            return Task.FromResult<Result>(new ResultError(
                ResultErrorType.Exception,
                $"Exception thrown when attempting to store {nameof(T)}",
                e));
        }
    }

    public Task<Result> RemoveValue(string key)
    {
        try
        {
            memoryCache.Remove(key);
            return Task.FromResult(new Result());
        }
        catch (Exception e)
        {
            return Task.FromResult<Result>(new ResultError(
                ResultErrorType.Exception,
                $"Exception thrown when attempting to remove \"{key}\" from cache",
                e));
        }
    }
}