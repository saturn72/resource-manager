#region

using System;

#endregion

namespace QAutomation.Core.Services.Caching
{
    public static class CacheManagerExtensions
    {
        public const uint DefaultCacheTime = 60;


        /// <summary>
        ///     Add datato cache
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="key">Cahce key</param>
        /// <param name="value">cached data</param>
        public static void Set<TCachedObject>(this ICacheManager cacheManager, string key, TCachedObject value)
        {
            cacheManager.Set<TCachedObject>(key, value, DefaultCacheTime);
        }

        /// <summary>
        ///     Addsite to cache only if not already exists
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void SetIfNotExists<TCachedObject>(this ICacheManager cacheManager, string key, TCachedObject value)
        {
            SetIfNotExists(cacheManager, key, value, DefaultCacheTime);
        }

        /// <summary>
        ///     Addsite to cache only if not already exists
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTimeInMinutes"></param>
        public static void SetIfNotExists<TCachedObject>(ICacheManager cacheManager, string key, TCachedObject value, uint cacheTimeInMinutes)
        {
            if (cacheManager.Get<TCachedObject>(key) != null)
                return;
            cacheManager.Set(key, value, cacheTimeInMinutes);
        }

        /// <summary>
        ///     Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="TCachedObject">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static TCachedObject Get<TCachedObject>(this ICacheManager cacheManager, string key,
            Func<TCachedObject> acquire)
        {
            return Get(cacheManager, key, acquire, DefaultCacheTime);
        }
        /// <summary>
         ///     Get a cached item. If it's not in the cache yet, then load and cache it
         /// </summary>
         /// <typeparam name="TCachedObject">Type</typeparam>
         /// <param name="cacheManager">Cache manager</param>
         /// <param name="key">Cache key</param>
         /// <param name="acquire">Function to load item if it's not in the cache yet</param>
         /// <returns>Cached item</returns>
            public static TCachedObject Get<TCachedObject>(this ICacheManager cacheManager, string key, Func<TCachedObject> acquire, uint cacheTime)
        {
            var cachedObject = cacheManager.Get<TCachedObject>(key);
            if (cachedObject != null)
                return cachedObject;
            cachedObject = acquire();
            cacheManager.Set(key, cachedObject, cacheTime);
            return cachedObject;
        }
    }
}