using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace StupidChat.Application.Common.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task<T?> GetObjectAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        where T : class
    {
        var cachedBytes = await cache.GetAsync(key, cancellationToken);
        return cachedBytes == null ? null : JsonSerializer.Deserialize<T>(cachedBytes);
    }

    public static async Task SetObjectAsync<T>(this IDistributedCache cache, string key, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        where T : class
    {
        var cachedBytes = JsonSerializer.SerializeToUtf8Bytes(data);
        await cache.SetAsync(key, cachedBytes, options, cancellationToken);
    }
}