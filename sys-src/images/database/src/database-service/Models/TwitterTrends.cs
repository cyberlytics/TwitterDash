using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseService.Models
{
    public class TwitterTrends
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateTime { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Country { get; set; }
        public List<TwitterTrend> Trends { get; set; }
    }
}