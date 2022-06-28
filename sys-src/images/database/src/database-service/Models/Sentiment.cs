using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseService.Models
{
    public class Sentiment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        public long Tweet_ID { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Timestamp { get; set; }
        public string Trend { get; set; }
        public float Value { get; set; }
    }
}