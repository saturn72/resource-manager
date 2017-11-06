#region

using System;
using Microsoft.Extensions.Caching.Memory;

#endregion

namespace QAutomation.Core.Services.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        #region Fields
        private readonly IMemoryCache _memoryCache;

        #endregion
        #region ctor
        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        #endregion

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public TCachedObject Get<TCachedObject>(string key)
        {
            TCachedObject value;
            return _memoryCache.TryGetValue(key, out value) ?
            value : default(TCachedObject);
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Set<TCachedObject>(string key, TCachedObject value, int cacheTime)
        {
            _memoryCache.Set(key, value, TimeSpan.FromSeconds(cacheTime));
        }
    }
}