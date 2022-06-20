using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Twitterdash;
using DatabaseService.Repositories;
using DatabaseService.Models;

namespace DatabaseService.Controller
{
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
            TwitterTrends? reply;

            Console.WriteLine(request);

            if (request.HasCountry)
            {
                reply = await repository.GetCurrentTrends(this.Woeid.getWOEID(request.Country));
            }
            else
            {
                reply = await repository.GetCurrentTrends(null);
            }

            if (reply == null)
            {
                return new TrendProviderReply();
            }

            var replyTrends = new List<Trend>();

            foreach (var trend in reply.Trends)
            {
                var T = new Trend();
                T.TrendType = (Twitterdash.TrendType)trend.trendType;
                T.Country = trend.woeid;
                T.Name = trend.name;
                T.Placement = trend.placement;
                T.TweetVolume24 = trend.tweetVolume24;

                replyTrends.Add(T);
            }

            replyTrends = replyTrends.OrderBy(x => x.Placement).Take(request.Limit).ToList();

            var trendproviderreply = new TrendProviderReply();
            trendproviderreply.Timestamp = reply.DateTime.ToUniversalTime().ToTimestamp();
            trendproviderreply.Trends.Add(replyTrends);

            return trendproviderreply;
        }

        public override async Task<GetRecentTrendsReply> GetRecentTrends(GetRecentTrendsRequest request, ServerCallContext context)
        {
            var db_reply = await repository.GetRecentTrends(
                request.EndDate?.ToDateTime(),
                request.StartDate?.ToDateTime(),
                request.Hashtag);


            var recentTrends = new List<RecentTrend>();

            foreach (var reply in db_reply)
            {
                var recentTrend = new RecentTrend();
                recentTrend.Datetime = reply.DateTime.ToUniversalTime().ToTimestamp();
                recentTrend.Trend = new();

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
            getRecentTrendsReply.RecentTrends.Add(recentTrends);

            return getRecentTrendsReply;
        }
    }
}