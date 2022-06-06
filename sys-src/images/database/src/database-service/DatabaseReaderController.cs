using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Twitterdash;
using MongoDB.Driver;
using places;
using MongoDB.Bson;

internal class DatabaseReaderController : Twitterdash.DatabaseReader.DatabaseReaderBase
{
    
    private MongoClient client;
    private woeid Woeid;    

    public DatabaseReaderController(MongoClient client, woeid Woeid)
    {
        this.client = client;
        this.Woeid = Woeid;
    }

    public override async Task<GetAvailableCountriesReply> GetAvailableCountries(Empty request, ServerCallContext context)
    {
        var database = client.GetDatabase("TwitterDash");
        var collection = database.GetCollection<TwitterTrends>("Trends");
        
        var filter = new BsonDocument();
        var countries = await collection.DistinctAsync<int>("country", filter);


        var getAvailableCountriesReply = new GetAvailableCountriesReply();

        for (int i = 0; i < countries.ToList().Count; i++)
        {
            getAvailableCountriesReply.Countries.Add(this.Woeid.getCountry(countries.ToList()[i]));
        }

        return getAvailableCountriesReply;
    }

    public override async Task<TrendProviderReply> GetCurrentTrends(GetCurrentTrendsRequest request, ServerCallContext context)
    {
              
        var database = client.GetDatabase("TwitterDash");
        var collection = database.GetCollection<TwitterTrends>("Trends");

        //Todo: Last or first???
        var db_reply = (await collection.FindAsync(t => t.Country == this.Woeid.getWOEID(request.Country))).ToList().Last();

        var replyTrends = new List<Trend>();

        foreach (var trend in db_reply.Trends)
        {
            var T = new Trend();
            T.TrendType = (Twitterdash.TrendType)trend.trendType;
            T.Country = trend.woeid;
            T.Name = trend.name;
            T.Placement = trend.placement;
            T.TweetVolume24 = trend.tweetVolume24;

            replyTrends.Add(T);
        }

        var trendproviderreply = new TrendProviderReply();
        trendproviderreply.Timestamp = db_reply.DateTime.ToUniversalTime().ToTimestamp();
        trendproviderreply.Trends.AddRange(replyTrends);

        return trendproviderreply;
    }
    
    public override async Task<GetRecentTrendsReply> GetRecentTrends(GetRecentTrendsRequest request, ServerCallContext context)
    {
        //request.Hashtag;


        var database = client.GetDatabase("TwitterDash");
        var collection = database.GetCollection<TwitterTrends>("Trends");

        var db_reply = (await collection.FindAsync(x => x.DateTime.Date < request.EndDate.ToDateTime() && x.DateTime.Date > request.StartDate.ToDateTime() && x.Trends.Any(y => y.name == request.Hashtag))).ToList();


        var recentTrends = new List<RecentTrend>();

        foreach (var reply in db_reply)
        {
            var recentTrend = new RecentTrend();
            recentTrend.Datetime = reply.DateTime.ToUniversalTime().ToTimestamp();

            foreach (var trend in reply.Trends)
            {
                if (trend.name == request.Hashtag)
                {
                    recentTrend.Trend.TrendType = (Twitterdash.TrendType)trend.trendType;
                    recentTrend.Trend.Country = trend.woeid;
                    recentTrend.Trend.Name = trend.name;
                    recentTrend.Trend.Placement = trend.placement;
                    recentTrend.Trend.TweetVolume24 = trend.tweetVolume24;
                    break;
                }
            }
            recentTrends.Add(recentTrend);
        }

        var getRecentTrendsReply = new GetRecentTrendsReply();
        getRecentTrendsReply.RecendTrends.AddRange(recentTrends);
        
        return getRecentTrendsReply;
    }
}