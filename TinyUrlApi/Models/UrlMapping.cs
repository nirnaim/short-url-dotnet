using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TinyUrlApi.Models
{
    public class UrlMapping
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("ShortCode")]
        public string ShortCode { get; set ; } = null!;

        [BsonElement("LongUrl")]
        public string LongUrl { get; set; } = null!;
    }
}
