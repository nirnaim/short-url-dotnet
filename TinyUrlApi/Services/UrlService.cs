using TinyUrlApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace TinyUrlApi.Services
{
    public class UrlService
    {
        private readonly IMongoCollection<UrlMapping> _urlMappingsCollection;

        public UrlService(IOptions<TinyUrlDatabaseSettings> tinyUrlDatabaseSettings)
        {
            var mongoClient = new MongoClient(
            tinyUrlDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
            tinyUrlDatabaseSettings.Value.DatabaseName);

            _urlMappingsCollection = mongoDatabase.GetCollection<UrlMapping>(
            tinyUrlDatabaseSettings.Value.UrlMappingsCollectionName);
        }
        public async Task<List<UrlMapping>> GetAsync() =>
        await _urlMappingsCollection.Find(_ => true).ToListAsync();

        public async Task<UrlMapping?> GetAsync(string id) =>
            await _urlMappingsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<UrlMapping?> GetAsyncByShortCode(string shortCode) =>
            await _urlMappingsCollection.Find(x => x.ShortCode == shortCode).FirstOrDefaultAsync();

        public async Task CreateAsync(UrlMapping newUrlMapping) =>
            await _urlMappingsCollection.InsertOneAsync(newUrlMapping);

        public async Task UpdateAsync(string id, UrlMapping updatedUrlMapping) =>
            await _urlMappingsCollection.ReplaceOneAsync(x => x.Id == id, updatedUrlMapping);

        public async Task RemoveAsync(string id) =>
            await _urlMappingsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
