namespace TinyUrlApi.Models
{
    public class TinyUrlDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UrlMappingsCollectionName { get; set; } = null!;
    }
}
