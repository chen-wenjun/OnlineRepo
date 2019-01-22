using Raymark.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Raymark.Caching
{
    public class Cache
    {
        #region Private Fields

        private static Cache _current = new Cache();
        private TypeCache _serviceCache;

        // Default value from app config
        private TimeSpan _cachingSlidingExpiration = AppConfig.Instance.CachingSlidingExpiration;

        private object _posTypeCacheLock = new object();

        #endregion

        #region Constructors

        private Cache()
        {
            _serviceCache = new TypeCache(_cachingSlidingExpiration);
        }

        #endregion

        #region Public Members

		public static string CacheKeySeparator 
		{
			get
			{
				return ",";
			}
		}

        public static Cache Current
        {
            get
            {
                return _current;
            }
        }

        public long GetCountFromCache()
        {
            return _serviceCache.GetCountFromCache();
        }

        public long GetAllCount()
        {
            return _serviceCache.GetAllCount();
        }

        public long GetCount()
        {
            return _serviceCache.GetCount();
        }

        public long GetCountOfServiceCache(CacheTypeEnum cacheType)
        {
            return _serviceCache.GetCount(cacheType);
        }

        public long GetCountOfServiceCache()
        {
            return _serviceCache.GetCount();
        }

        public long GetCountOfTerminalCache(string storeCode, int registerNumber, CacheTypeEnum cacheType)
        {
            return _serviceCache.GetCount(BuildTerminalKey(storeCode, registerNumber), cacheType);
        }

        public long GetCountOfTerminalCache(string storeCode, int registerNumber)
        {
            return _serviceCache.GetCount(BuildTerminalKey(storeCode, registerNumber));
        }

        public long GetCountOfTerminalCache()
        {
            long count = 0L;
            return count;
        }

        public long GetCountOfStoreCache(string storeCode, CacheTypeEnum cacheType)
        {
            return _serviceCache.GetCount(storeCode, cacheType);
        }

        public long GetCountOfStoreCache(string storeCode)
        {
            return _serviceCache.GetCount(storeCode);
        }

        public long GetCountOfStoreCache()
        {
            long count = 0L;
            return count;
        }

        public List<string> GetKeys()
        {
            return _serviceCache.GetKeys();
        }

        public List<string> GetKeysOfServiceCache(CacheTypeEnum cacheType)
        {
            return _serviceCache.GetKeys(cacheType);
        }

        public List<string> GetKeysOfTerminalCache(string storeCode, int registerNumber, CacheTypeEnum cacheType)
        {
            return _serviceCache.GetKeys(BuildTerminalKey(storeCode, registerNumber), cacheType);
        }

        public List<string> GetKeysOfStoreCache(string storeCode, CacheTypeEnum cacheType)
        {
            return _serviceCache.GetKeys(storeCode, cacheType);
        }

        public List<CacheTypeEnum> GetTypesOfServiceCache()
        {
            return _serviceCache.GetTypes();
        }

        public List<CacheTypeEnum> GetTypesOfTerminalCache(string storeCode, int registerNumber)
        {
            return _serviceCache.GetTypes(BuildTerminalKey(storeCode, registerNumber));
        }

        public List<CacheTypeEnum> GetTypesOfStoreCache(string storeCode)
        {
            return _serviceCache.GetTypes(storeCode);
        }

        public List<string> GetStores()
        {
            return new List<string>();
        }

        public Dictionary<string, List<int>> GetTerminals()
        {
            Dictionary<string, List<int>> terminals = new Dictionary<string, List<int>>();
            return terminals;
        }

        public T Get<T>(CacheTypeEnum cacheType, string key) where T : class
        {
            return _serviceCache.Get<T>(cacheType, key);
        }

        public object Get(CacheTypeEnum cacheType, string key)
        {
            return _serviceCache.Get(cacheType, key);
        }

        public T Get<T>(string storeCode, int registerNumber, CacheTypeEnum cacheType, string key) where T : class
        {
            return _serviceCache.Get<T>(BuildTerminalKey(storeCode, registerNumber), cacheType, key);
        }

        public object Get(string storeCode, int registerNumber, CacheTypeEnum cacheType, string key)
        {
            return _serviceCache.Get(BuildTerminalKey(storeCode, registerNumber), cacheType, key);
        }

        public T Get<T>(string storeCode, CacheTypeEnum cacheType, string key) where T : class
        {
            return _serviceCache.Get<T>(storeCode, cacheType, key);
        }

        public object Get(string storeCode, CacheTypeEnum cacheType, string key)
        {
            return _serviceCache.Get(storeCode, cacheType, key);
        }

        public void Add(CacheTypeEnum cacheType, string key, object data)
        {
            _serviceCache.Add(cacheType, key, data);
        }

        public void Add(CacheTypeEnum cacheType, string key, object data, TimeSpan slidingExpiration)
        {
            _serviceCache.Add(cacheType, key, data, slidingExpiration);
        }

        public void Add(CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration)
        {
            _serviceCache.Add(cacheType, key, data, absoluteExpiration);
        }

        public void Add(string storeCode, int registerNumber, CacheTypeEnum cacheType, string key, object data)
        {
            AddTerminalCache(storeCode, registerNumber, cacheType, key, data, null, null);
        }

        public void Add(string storeCode, int registerNumber, CacheTypeEnum cacheType, string key, object data, TimeSpan slidingExpiration)
        {
            AddTerminalCache(storeCode, registerNumber, cacheType, key, data, slidingExpiration, null);
        }

        public void Add(string storeCode, int registerNumber, CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration)
        {
            AddTerminalCache(storeCode, registerNumber, cacheType, key, data, null, absoluteExpiration);
        }

        public void Add(string storeCode, CacheTypeEnum cacheType, string key, object data)
        {
            AddStoreCache(storeCode, cacheType, key, data, null, null);
        }

        public void Add(string storeCode, CacheTypeEnum cacheType, string key, object data, TimeSpan slidingExpiration)
        {
            AddStoreCache(storeCode, cacheType, key, data, slidingExpiration, null);
        }

        public void Add(string storeCode, CacheTypeEnum cacheType, string key, object data, DateTime absoluteExpiration)
        {
            AddStoreCache(storeCode, cacheType, key, data, null, absoluteExpiration);
        }

        public void RemoveAll()
        {
            this.Remove();

        }

        public void Remove()
        {
            _serviceCache.Remove();
        }

        public void Remove(CacheTypeEnum cacheType)
        {
            _serviceCache.Remove(cacheType);
        }

        public void Remove(CacheTypeEnum cacheType, string key)
        {
            _serviceCache.Remove(cacheType, key);
        }

        public void Remove(string storeCode, int registerNumber)
        {
            _serviceCache.Remove(BuildTerminalKey(storeCode, registerNumber));
        }

        public void Remove(string storeCode, int registerNumber, CacheTypeEnum cacheType)
        {
            _serviceCache.Remove(BuildTerminalKey(storeCode, registerNumber), cacheType);
        }

        public void Remove(string storeCode, int registerNumber, CacheTypeEnum cacheType, string key)
        {
            _serviceCache.Remove(BuildTerminalKey(storeCode, registerNumber), cacheType, key);
        }

        public void Remove(string storeCode)
        {
            _serviceCache.Remove(storeCode);
        }

        public void Remove(string storeCode, CacheTypeEnum cacheType)
        {
            _serviceCache.Remove(storeCode, cacheType);
        }

        public void Remove(string storeCode, CacheTypeEnum cacheType, string key)
        {
            _serviceCache.Remove(storeCode, cacheType, key);
        }

        #endregion

        #region Private Members

        private void AddTerminalCache(string storeCode, int registerNumber, CacheTypeEnum cacheType, string key, object data, TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
        {
            string terminalKey = BuildTerminalKey(storeCode, registerNumber);

            if (slidingExpiration.HasValue && absoluteExpiration.HasValue)
                _serviceCache.Add(terminalKey, cacheType, key, data, absoluteExpiration.Value, slidingExpiration.Value);
            else if (slidingExpiration.HasValue)
                _serviceCache.Add(terminalKey, cacheType, key, data, slidingExpiration.Value);
            else if (absoluteExpiration.HasValue)
                _serviceCache.Add(terminalKey, cacheType, key, data, absoluteExpiration.Value);
            else
                _serviceCache.Add(terminalKey, cacheType, key, data);
        }

        private void AddStoreCache(string storeCode, CacheTypeEnum cacheType, string key, object data, TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
        {
            if (slidingExpiration.HasValue && absoluteExpiration.HasValue)
                _serviceCache.Add(storeCode, cacheType, key, data, absoluteExpiration.Value, slidingExpiration.Value);
            else if (slidingExpiration.HasValue)
                _serviceCache.Add(storeCode, cacheType, key, data, slidingExpiration.Value);
            else if (absoluteExpiration.HasValue)
                _serviceCache.Add(storeCode, cacheType, key, data, absoluteExpiration.Value);
            else
                _serviceCache.Add(storeCode, cacheType, key, data);
        }

        private static string BuildTerminalKey(string storeCode, int registerNumber)
        {
            return string.Format("{0}&{1}", storeCode, registerNumber);
        }

        private static void ParseTerminalKey(string terminalKey, out string storeCode, out int registerNumber)
        {
            string[] arr = terminalKey.Split('&');
            if (arr.Length != 2)
                throw new Exception("Cannot parse terminal key. It's not in correct format.");

            storeCode = arr[0];
            registerNumber = Convert.ToInt32(arr[1]);
        }

        #endregion
    }
}
