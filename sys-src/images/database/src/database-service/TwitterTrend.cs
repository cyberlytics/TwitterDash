using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

public class TwitterTrend
{
    public TrendType trendType;
    public string name;
    public int woeid;
    public int placement;
    public int tweetVolume24;
}

public enum TrendType
{
    Topic,
    Hashtag
}