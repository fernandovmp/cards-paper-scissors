using System.Collections.Concurrent;

namespace FernandoVmp.GodotUtils.Services;

public class MemoryCacheService
{
    private static ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();

    public void AddOrReplace(string key, object value) => _dictionary.AddOrUpdate(key, value, (s, o) => value);

    public T? GetValueOrDefault<T>(string key)
    {
        if (_dictionary.TryGetValue(key, out object? value))
        {
            return (T)value;
        }

        return default;
    }
}