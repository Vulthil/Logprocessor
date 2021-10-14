using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogProcessor.Models;
using Microsoft.Extensions.Caching.Memory;

namespace LogProcessor.Services
{
    public class ConfiguredMemoryCache : IConfiguredMemoryCache
    {
        private readonly IMemoryCache _cache;
        private int _slidingWindow = 12;
        public ConfiguredMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task CacheError(ErrorResult error)
        {
            return _cache.GetOrCreateAsync(error.Message.SessionId, c =>
            {
                //TODO: Decide whether to use sliding window or absolute expiration.
                c.SlidingExpiration = TimeSpan.FromHours(_slidingWindow);
                //c.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                return Task.FromResult(new object());
            });
        }

        public void Dispose()
        {
            
        }

        public ICacheEntry CreateEntry(object key)
        {
            return _cache.CreateEntry(key);
        }

        public void Remove(object key)
        {
            _cache.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _cache.TryGetValue(key, out value);
        }
    }
}
