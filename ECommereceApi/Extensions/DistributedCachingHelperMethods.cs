using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ECommereceApi.Extensions
{
    public static class DistributedCachingHelperMethods
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string recordKey,
            T recordValue,
            TimeSpan? absoluteExpiryDate = null,
            TimeSpan? unusedExpryDate = null)
        {
            var options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = absoluteExpiryDate ?? TimeSpan.FromSeconds(60);
            options.SlidingExpiration = unusedExpryDate;

            var JsonValue = JsonSerializer.Serialize<T>(recordValue);
            await cache.SetStringAsync(recordKey, JsonValue, options);
        }
        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordKey)
        {
            var JsonValue = await cache.GetStringAsync(recordKey);
            if(JsonValue is null)
                return default(T);
            return JsonSerializer.Deserialize<T>(JsonValue);
        }
    }
}
