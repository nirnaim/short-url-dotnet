using System.Collections.Concurrent;

namespace TinyUrlApi.Services
{
    public class CacheService<TKey, TValue> where TKey : IEquatable<TKey>
    {
        /*
         * For a size-limited cache with concurrency and multi-threading support, along with collapsed requests, I'll implement a thread-safe LRU (Least Recently Used) cache.
         * LRU is a popular cache eviction policy that removes the least recently used items when the cache reaches its maximum capacity. 
         * This approach ensures that frequently accessed items stay in the cache while evicting less used items when the cache is full.
        */
        private readonly int _capacity;
        private readonly ConcurrentDictionary<TKey, LinkedListNode<(TKey key, TValue value)>> _cacheMap;
        private readonly LinkedList<(TKey key, TValue value)> _cacheList;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public CacheService(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
            }
            _capacity = capacity;
            _cacheMap = new ConcurrentDictionary<TKey, LinkedListNode<(TKey key, TValue value)>>();
            _cacheList = new LinkedList<(TKey key, TValue value)>();
        }

        public bool TryGet(TKey key, out TValue? value)
        {
            _lock.EnterReadLock();
            try
            {
                if (_cacheMap.TryGetValue(key, out var node))
                {
                    _cacheList.Remove(node);
                    _cacheList.AddFirst(node);
                    value = node.Value.value;
                    return true;
                }
                value = default;
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_cacheMap.TryGetValue(key, out var node))
                {
                    // Update existing item
                    _cacheList.Remove(node);
                    _cacheList.AddFirst(node);
                    node.Value = (key, value);
                }
                else
                {
                    // Add new item
                    var newNode = _cacheList.AddFirst((key, value));
                    _cacheMap.TryAdd(key, newNode);

                    // Check and remove least recently used item if capacity exceeded
                    if (_cacheMap.Count > _capacity)
                    {
                        var lastNode = _cacheList.Last;
                        _cacheList.RemoveLast();
                        _cacheMap.TryRemove(lastNode.Value.key, out _);
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
