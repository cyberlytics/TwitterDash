using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Twitterdash;
using places;

internal class DatabaseReaderController : Twitterdash.DatabaseReader.DatabaseReaderBase
{

    private woeid Woeid;
    private ITwitterTrendsRepository repository;

    public DatabaseReaderController(ITwitterTrendsRepository repository, woeid Woeid)
    {
        this.repository = repository;
        this.Woeid = Woeid;
    }

    public override async Task<GetAvailableCountriesReply> GetAvailableCountries(Empty request, ServerCallContext context)
    {
        var countries = await repository.GetAvailableCountries();
        var getAvailableCountriesReply = new GetAvailableCountriesReply();

        foreach (int woeid in countries)
        {
            getAvailableCountriesReply.Countries.Add(this.Woeid.getCountry(woeid));
        }
        return getAvailableCountriesReply;
    }

    public override async Task<TrendProviderReply> GetCurrentTrends(GetCurrentTrendsRequest request, ServerCallContext context)
    {
        var db_reply = await repository.GetCurrentTrends(this.Woeid.getWOEID(request.Country));

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
        var db_reply = await repository.GetRecentTrends(
            request.EndDate.ToDateTime(),
            request.StartDate.ToDateTime(),
            request.Hashtag);


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