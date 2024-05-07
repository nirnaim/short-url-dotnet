using TinyUrlApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Text;
using System.Security.Cryptography;

namespace TinyUrlApi.Services
{
    public class UrlService
    {
        private const int MaxCacheSize = 100;
        private readonly IMongoCollection<UrlMapping> _urlMappingsCollection;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;
        private readonly CacheService<string, UrlMapping> _cacheService;

        public UrlService(IOptions<TinyUrlDatabaseSettings> tinyUrlDatabaseSettings)
        {
            var mongoClient = new MongoClient(
            tinyUrlDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
            tinyUrlDatabaseSettings.Value.DatabaseName);

            _urlMappingsCollection = mongoDatabase.GetCollection<UrlMapping>(
            tinyUrlDatabaseSettings.Value.UrlMappingsCollectionName);

            _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
            _cacheService = new CacheService<string, UrlMapping>(MaxCacheSize);
        }

        public async Task<UrlMapping?> GetAsyncByShortCode(string shortCode) =>
            await _urlMappingsCollection.Find(x => x.ShortCode == shortCode).FirstOrDefaultAsync();

        public async Task<UrlMapping?> CreateOrGetAsync(string longUrl)
        {
            for (int salt = 1; salt < 1000; salt++)
            {
                var shortCode = GenerateShortCode(longUrl, salt);
                var urlMapping = await GetAsyncCachedByShortCode(shortCode);
                if (urlMapping == null)
                {
                    var newUrlMapping = new UrlMapping { LongUrl = longUrl, ShortCode = shortCode };
                    await _urlMappingsCollection.InsertOneAsync(newUrlMapping);
                    return newUrlMapping;
                }
                else if (urlMapping.LongUrl == longUrl)
                {
                    return urlMapping;
                }
            }
            return null;
        }
            

        public async Task<UrlMapping?> GetAsyncCachedByShortCode(string shortCode)
        {
            if (_cacheService.TryGet(shortCode, out var cachedUrlMapping))
            {
                return cachedUrlMapping;
            }
            var semaphore = _locks.GetOrAdd(shortCode, _ => new SemaphoreSlim(1));
            await semaphore.WaitAsync();
            if (_cacheService.TryGet(shortCode, out cachedUrlMapping))
            {
                semaphore.Release();
                return cachedUrlMapping;
            }
            try
            {
                var urlMapping = await GetAsyncByShortCode(shortCode);
                if (urlMapping == null)
                {
                    return null;
                }
                _cacheService.AddOrUpdate(shortCode, urlMapping);
                return urlMapping;
            }
            finally
            {
                semaphore.Release();
                _locks.TryRemove(shortCode, out _);
            }
        }

        public string GenerateShortCode(string longUrl, int salt, int desiredLength = 8)
        {
            string hash;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(longUrl + salt);
                byte[] hashBytes = sha256Hash.ComputeHash(bytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
           return hash.Substring(0, desiredLength);
        }

        public async Task<bool> CheckUrlMapping(string shortCode, string longUrl) 
        {
            var urlMapping = await GetAsyncCachedByShortCode(shortCode);
            if (urlMapping == null)
            {
                return true;
            }
            return longUrl == urlMapping.LongUrl;
        }
    }
}
