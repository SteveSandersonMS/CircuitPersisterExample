using Microsoft.Extensions.Caching.Memory;

namespace CircuitPersister;

/// <summary>
/// This is responsible for actually saving the data from the circuits. In this example it just uses MemoryCache
/// but if you can be sure all your data is serializable, you could put it in Redis or similar, and then it would
/// outlive server changes/restarts. However you would then also be responsible for versioning your data to avoid
/// loading state that's incompatible with newer versions of your code.
/// </summary>
public class CircuitStateStore
{
    private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    public Task<Dictionary<string, object?>> LoadAsync(Guid persistenceId)
    {
        var result = _memoryCache.TryGetValue(persistenceId, out Dictionary<string, object?>? oldDict) ? oldDict! : new();
        return Task.FromResult(result);
    }

    public Task SaveAsync(Guid persistenceId, Dictionary<string, object?> values)
    {
        _memoryCache.Set(persistenceId, values);
        return Task.CompletedTask;
    }
}
