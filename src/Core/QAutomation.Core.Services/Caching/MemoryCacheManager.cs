#region

using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

#endregion

namespace QAutomation.Core.Services.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        #region ctor

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #endregion

        public void Clear()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested &&
                _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();
        }

        public TCachedObject Get<TCachedObject>(string key)
        {
            TCachedObject value;
            return _memoryCache.TryGetValue(key, out value) ? value : default(TCachedObject);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void Set<TCachedObject>(string key, TCachedObject value, int cacheTime)
        {
            _memoryCache.Set(key, value, TimeSpan.FromSeconds(cacheTime));
        }

        #region Fields

        private readonly IMemoryCache _memoryCache;
        private CancellationTokenSource _resetCacheToken;

        #endregion
    }
}