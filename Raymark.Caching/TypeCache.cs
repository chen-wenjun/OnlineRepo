using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime;
using System.Runtime.Caching;

namespace Raymark.Caching
{
    /// <summary>
    /// Internal Cache structure containing all types of data
    /// </summary>
    internal class TypeCache
    {
        #region Private Fields

        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Link region -> cacheType -> cacheKey 1 -> n relationship
        private ConcurrentDictionary<string, ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>>> _typeCacheKeys = new ConcurrentDictionary<string, ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>>>();
        private static MemoryCache _cache = MemoryCache.Default;
        private const string _GLOBAL = "__g__";
        private static int _evictCount = 0;
        private static int _evictCountGC = 100000;

        // The following fields override MemoryCache default policy settins if provided.
        private DateTime? _absoluteExpiration;
        private TimeSpan? _slidingExpiration;
        private CacheItemPriority? _priority;

        private static object _typeCacheLock = new object();

        #endregion

        #region Constructors

        public TypeCache()
        {
            string evictCountConfig = ConfigurationManager.AppSettings["cachingEvictCountGC"];
            int evictCountGC;
            if(!string.IsNullOrWhiteSpace(evictCountConfig) && int.TryParse(evictCountConfig, out evictCountGC))
            {
                _evictCountGC = evictCountGC;
            }

            _logger.InfoFormat("[MemoryCache.Default.CacheMemoryLimit]={0}, [MemoryCache.Default.PhysicalMemoryLimit]={1}, [MemoryCache.Default.PollingInterval]={2}, [cachingEvictCountGC]={3}", 
                MemoryCache.Default.CacheMemoryLimit, MemoryCache.Default.PhysicalMemoryLimit, MemoryCache.Default.PollingInterval, _evictCountGC);
        }

        public TypeCache(TimeSpan slidingExpiration) : this()
        {
            _slidingExpiration = slidingExpiration;
        }

        public TypeCache(DateTime absoluteExpiration) : this()
        {
            _absoluteExpiration = absoluteExpiration;
        }

        public TypeCache(DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority) : this()
        {
            _absoluteExpiration = absoluteExpiration;
            _slidingExpiration = slidingExpiration;
            _priority = priority;
        }

        #endregion

        #region Public Methods

        public long GetCount(CacheTypeEnum cacheType)
        {
            return GetCount(_GLOBAL, cacheType);
        }

        public long GetCount(string region, CacheTypeEnum cacheType)
        {
            long count = 0L;

            ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>> typeKeys;
            if (_typeCacheKeys.TryGetValue(region, out typeKeys))
            {
                ConcurrentDictionary<string, byte> keys;
                if (typeKeys.TryGetValue(cacheType, out keys))
                {
                    count += keys.Count;
                }
            }

            return count;
        }

        public long GetCountFromCache()
        {
            return _cache.GetCount();
        }

        public long GetCount()
        {
            return GetCount(_GLOBAL);
        }

        public long GetAllCount()
        {
            long count = 0L;

            foreach(var region in _typeCacheKeys.Keys)
            {
                count += GetCount(region);
            }

            return count;
        }

        public long GetCount(string region)
        {
            long count = 0L;

            ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>> typeKeys;
            if (_typeCacheKeys.TryGetValue(region, out typeKeys))
            {
                foreach (CacheTypeEnum type in typeKeys.Keys)
                {
                    ConcurrentDictionary<string, byte> keys;
                    if (typeKeys.TryGetValue(type, out keys))
                    {
                        count += keys.Count;
                    }
                }
            }

            return count;
        }

        public List<string> GetKeys()
        {
            List<string> cacheKeys = new List<string>();

            foreach (var regionPair in _typeCacheKeys)
            {
                foreach(var typePair in regionPair.Value)
                {
                    cacheKeys.AddRange(GetKeys(regionPair.Key, typePair.Key));
                }
            }

            return cacheKeys;
        }

        public List<string> GetKeys(CacheTypeEnum cacheType)
        {
            return GetKeys(_GLOBAL, cacheType);
        }

        /// <summary>
        /// Get list of all keys for specific cache type
        /// </summary>
        /// <param name="cacheType"></param>
        /// <returns></returns>
        public List<string> GetKeys(string region, CacheTypeEnum cacheType)
        {
            List<string> cacheKeys = new List<string>();

            ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>> typeKeys;
            if(_typeCacheKeys.TryGetValue(region, out typeKeys))
            {
                ConcurrentDictionary<string, byte> keys;
                if(typeKeys.TryGetValue(cacheType, out keys))
                {
                    cacheKeys.AddRange(keys.Keys);
                }
            }

            return cacheKeys;
        }

        /// <summary>
        /// Get list of all cache types
        /// </summary>
        /// <returns></returns>
        public List<CacheTypeEnum> GetTypes()
        {
            return GetTypes(_GLOBAL);
        }

        public List<CacheTypeEnum> GetTypes(string region)
        {
            var cacheTypes = new List<CacheTypeEnum>();

            ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>> typeKeys;
            if (_typeCacheKeys.TryGetValue(region, out typeKeys))
            {
                cacheTypes.AddRange(typeKeys.Keys);
            }

            return cacheTypes;
        }

        public T Get<T>(CacheTypeEnum cacheType, string key) where T : class
        {
            return Get<T>(_GLOBAL, cacheType, key);
        }

        public T Get<T>(string region, CacheTypeEnum cacheType, string key) where T : class
        {
            var value = _cache.Get(BuildCacheKey(region, cacheType, key));
            return value != null && value is T ? (T)value : null;
        }

        public object Get(CacheTypeEnum cacheType, string key)
        {
            return Get(_GLOBAL, cacheType, key);
        }

        public object Get(string region, CacheTypeEnum cacheType, string key)
        {
            return _cache.Get(BuildCacheKey(region, cacheType, key));
        }

        public void Add(CacheTypeEnum cacheType, string key, object data)
        {
            Add(_GLOBAL, cacheType, key, data);
        }

        /// <summary>
        /// Add cache item to its type cache using policy specified in constructor or default if not spedified
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Add(string region, CacheTypeEnum cacheType, string key, object data)
        {
            CacheItemPolicy policy = new CacheItemPolicy();

            if (_slidingExpiration.HasValue)
                policy.SlidingExpiration = _slidingExpiration.Value;

            if (_absoluteExpiration.HasValue)
                policy.AbsoluteExpiration = _absoluteExpiration.Value;

            if (_priority.HasValue)
                policy.Priority = _priority.Value;

            Add(region, cacheType, key, data, policy);
        }

        public void Add(CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration)
        {
            Add(_GLOBAL, cacheType, key, data, absoluteExpiration);
        }

        public void Add(string region, CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;

            if (_slidingExpiration.HasValue)
                policy.SlidingExpiration = _slidingExpiration.Value;

            if (_priority.HasValue)
                policy.Priority = _priority.Value;

            Add(region, cacheType, key, data, policy);

        }

        public void Add(CacheTypeEnum cacheType, string key, object data, TimeSpan slidingExpiration)
        {
            Add(_GLOBAL, cacheType, key, data, slidingExpiration);
        }

        public void Add(string region, CacheTypeEnum cacheType, string key, object data, TimeSpan slidingExpiration)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = slidingExpiration;

            if (_absoluteExpiration.HasValue)
                policy.AbsoluteExpiration = _absoluteExpiration.Value;

            if (_priority.HasValue)
                policy.Priority = _priority.Value;

            Add(region, cacheType, key, data, policy);
        }

        public void Add(CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            Add(_GLOBAL, cacheType, key, data, absoluteExpiration, slidingExpiration);
        }

        public void Add(string region, CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = slidingExpiration;
            policy.AbsoluteExpiration = absoluteExpiration;

            if (_priority.HasValue)
                policy.Priority = _priority.Value;

            Add(region, cacheType, key, data, policy);
        }

        public void Add(CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority)
        {
            Add(_GLOBAL, cacheType, key, data, absoluteExpiration, slidingExpiration, priority);
        }

        public void Add(string region, CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;
            policy.SlidingExpiration = slidingExpiration;
            policy.Priority = priority;

            Add(region, cacheType, key, data, policy);
        }

        public void Add(CacheTypeEnum cacheType, string key, object data, CacheItemPolicy policy)
        {
            Add(_GLOBAL, cacheType, key, data, policy);
        }

        public void Add(string region, CacheTypeEnum cacheType, string key, object data, CacheItemPolicy policy)
        {
            policy.RemovedCallback += new CacheEntryRemovedCallback((CacheEntryRemovedArguments args) =>
            {
                _evictCount++;
                RemoveCacheKey(args.CacheItem.Key);

                if (_evictCount > _evictCountGC)
                {
                    lock (_typeCacheLock)
                    {
                        if (_evictCount > _evictCountGC)
                        {
                            _evictCount = 0;

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            _logger.InfoFormat("GC.Collect is called. [Total Cache Count]={0}, [Total Reg Keys]={1}", _cache.GetCount(), GetAllCount());
                        }
                    }
                }
            });

            string cacheKey = BuildCacheKey(region, cacheType, key);
            if (_cache.Add(cacheKey, data, policy))
            {
                AddCacheKey(region, cacheType, cacheKey);
            }
        }

        private void RemoveCacheKey(string cacheKey)
        {
            string[] keyPieces = cacheKey.Split('^');
            string region = _GLOBAL;
            CacheTypeEnum cacheType = (CacheTypeEnum)1000;
            string key;

            if(keyPieces.Length == 2)
            {
                cacheType = (CacheTypeEnum)Convert.ToInt32(keyPieces[0]);
                key = keyPieces[1];
            }
            else if(keyPieces.Length == 3)
            {
                region = keyPieces[0];
                cacheType = (CacheTypeEnum)Convert.ToInt32(keyPieces[1]);
                key = keyPieces[2];
            }

            ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>> typeKeys;
            if (_typeCacheKeys.TryGetValue(region, out typeKeys))
            {
                ConcurrentDictionary<string, byte> keys;
                if (typeKeys.TryGetValue(cacheType, out keys))
                {
                    byte value;
                    keys.TryRemove(cacheKey, out value);

                    if(keys.IsEmpty)
                    {
                        typeKeys.TryRemove(cacheType, out keys);

                        if(typeKeys.IsEmpty)
                        {
                            _typeCacheKeys.TryRemove(region, out typeKeys);
                        }
                    }
                }
            }
        }

        private void AddCacheKey(string region, CacheTypeEnum cacheType, string cacheKey)
        {
            _typeCacheKeys.AddOrUpdate(region, 
                (reg) =>
                {
                    var newKeyDic = new ConcurrentDictionary<string, byte>();
                    newKeyDic.GetOrAdd(cacheKey, new byte());

                    var newTypeDic = new ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>>();
                    newTypeDic.GetOrAdd(cacheType, newKeyDic);

                    return newTypeDic;
                }, 
                (reg, oldTypeDic) =>
                {

                    oldTypeDic.AddOrUpdate(cacheType, 
                        (type) =>
                        {
                            var newKeyDic = new ConcurrentDictionary<string, byte>();
                            newKeyDic.GetOrAdd(cacheKey, new byte());

                            return newKeyDic;
                        }, 
                        (type, oldKeyDic) =>
                        {
                            oldKeyDic.GetOrAdd(cacheKey, new byte());

                            return oldKeyDic;
                        });

                    return oldTypeDic;
                });
        }

        public void Remove(CacheTypeEnum cacheType, string key)
        {
            Remove(_GLOBAL, cacheType, key);
        }

        public void Remove(string region, CacheTypeEnum cacheType, string key)
        {
            _cache.Remove(BuildCacheKey(region, cacheType, key));
        }

        public void Remove(CacheTypeEnum cacheType)
        {
            Remove(_GLOBAL, cacheType);
        }

        public void Remove(string region, CacheTypeEnum cacheType)
        {
            ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>> typeKeys;
            if (_typeCacheKeys.TryGetValue(region, out typeKeys))
            {
                ConcurrentDictionary<string, byte> keys;
                if (typeKeys.TryGetValue(cacheType, out keys))
                {
                    foreach(KeyValuePair<string, byte> keyPair in keys)
                    {
                        _cache.Remove(keyPair.Key);
                    }
                }
            }
        }

        public void Remove()
        {
            Remove(_GLOBAL);
        }

        public void Remove(string region)
        {
            ConcurrentDictionary<CacheTypeEnum, ConcurrentDictionary<string, byte>> typeKeys;
            if (_typeCacheKeys.TryGetValue(region, out typeKeys))
            {
                foreach (CacheTypeEnum type in typeKeys.Keys)
                {
                    ConcurrentDictionary<string, byte> keys;
                    if (typeKeys.TryGetValue(type, out keys))
                    {
                        foreach (KeyValuePair<string, byte> keyPair in keys)
                        {
                            _cache.Remove(keyPair.Key);
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Members

        private static string BuildCacheKey(string region, CacheTypeEnum cacheType, string key)
        {
            if(region == _GLOBAL)
            {
                return string.Format("{0}^{1}", (int)cacheType, key);
            }
            else
            {
                return string.Format("{0}^{1}^{2}", region, (int)cacheType, key);
            }
        }

        #endregion
    }
}
