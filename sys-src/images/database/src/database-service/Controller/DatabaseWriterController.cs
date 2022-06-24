using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Twitterdash;
using DatabaseService.Repositories;
using DatabaseService.Models;

namespace DatabaseService.Controller
{
    public class DatabaseWriterController : Twitterdash.DatabaseWriter.DatabaseWriterBase
    {

        private ITwitterTrendsRepository trendRepository;
        private ISentimentRepository sentimentRepository;
        private woeid Woeid;

        public DatabaseWriterController(ITwitterTrendsRepository trendsRepository, ISentimentRepository sentimentRepository, woeid Woeid)
        {
            this.trendRepository = trendsRepository;
            this.sentimentRepository = sentimentRepository;
            this.Woeid = Woeid;
        }

        public override async Task<Empty> StoreTrends(TrendProviderReply request, ServerCallContext context)
        {
            var timestamp = request.Timestamp.ToDateTime();

            var Trends = new List<TwitterTrend>();
            foreach (var trend in request.Trends)
            {
                var twitterTrend = new TwitterTrend();
                twitterTrend.trendType = (DatabaseService.Models.TrendType)trend.TrendType;
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
            await trendRepository.StoreTrends(trends);

            return new Empty();
        }

        public override async Task<Empty> StoreSentiment(StoreSentimentRequest request, ServerCallContext context)
        {
            var sentiments = new List<Sentiment>();
            foreach (var payload in request.Sentiments)
            {
                var sentiment = new Sentiment();
                sentiment.Trend = payload.Topic;
                sentiment.Value = payload.Sentiment;
                sentiment.Tweet_ID = payload.Tweet.ID;
                sentiment.Timestamp = payload.Tweet.Timestamp.ToDateTime();
                sentiments.Add(sentiment);
            }
            await sentimentRepository.StoreSentiment(sentiments);
            return new Empty();
        }
    }
}