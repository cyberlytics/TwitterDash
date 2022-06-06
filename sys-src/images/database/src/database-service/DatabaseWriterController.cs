using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Twitterdash;
using MongoDB.Driver;
using places;

internal class DatabaseWriterController : Twitterdash.DatabaseWriter.DatabaseWriterBase
{

    private MongoClient client;
    private woeid Woeid;

    public DatabaseWriterController(MongoClient client, woeid Woeid)
    {
        this.client = client;
        this.Woeid = Woeid;
    }

    public override async Task<Empty> StoreTrends(TrendProviderReply request, ServerCallContext context)
    {        
        var database = client.GetDatabase("TwitterDash");
        var collection = database.GetCollection<TwitterTrends>("Trends");

        var timestamp = request.Timestamp.ToDateTime();

        var Trends = new List<TwitterTrend>();
        foreach (var trend in request.Trends)
        {
            var twitterTrend = new TwitterTrend();
            twitterTrend.trendType = (TrendType)trend.TrendType;
            twitterTrend.name = trend.Name;
            twitterTrend.woeid = trend.Country;
            twitterTrend.placement = trend.Placement;
            twitterTrend.tweetVolume24 = trend.TweetVolume24;

            Trends.Add(twitterTrend);
        }

        await collection.InsertOneAsync(new()
        {
            DateTime = timestamp,
            Country = Trends[0].woeid,
            Trends = Trends
        });

        return new Empty();
    }
}