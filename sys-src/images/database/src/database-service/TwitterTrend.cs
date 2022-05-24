using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class TwitterTrend
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime DateTime { get; set; }
    public Dictionary<string, int> Trends { get; set; }
}