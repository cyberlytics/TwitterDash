using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Twitterdash;
using places;

internal class DatabaseWriterController : Twitterdash.DatabaseWriter.DatabaseWriterBase
{

    private ITwitterTrendsRepository repository;
    private woeid Woeid;

    public DatabaseWriterController(ITwitterTrendsRepository repository, woeid Woeid)
    {
        this.repository = repository;
        this.Woeid = Woeid;
    }

    public override async Task<Empty> StoreTrends(TrendProviderReply request, ServerCallContext context)
    {
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

        var trends = new TwitterTrends
        {
            DateTime = timestamp,
            Country = Trends[0].woeid,
            Trends = Trends
        };
        await repository.StoreTrends(trends);

        return new Empty();
    }
}