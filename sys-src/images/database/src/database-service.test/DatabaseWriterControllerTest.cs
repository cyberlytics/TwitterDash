using DatabaseService.Controller;
using Google.Protobuf.WellKnownTypes;
using Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using Twitterdash;

namespace DatabaseService.Tests
{
    [TestFixture]
    class DatabaseWriterControllerTest : ControllerTestBase
    {
        [Test]
        public async Task StoreTrends_Should_Write_Trends_To_Database()
        {
            var service = new DatabaseWriterController(trendRepository, sentimentRepository, WOEID);

            var request = new Twitterdash.TrendProviderReply()
            {
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                Trends = { new Twitterdash.Trend{
                    TrendType = Twitterdash.TrendType.Hashtag,
                    Name = "TestTrend1",
                    Country = 23424829,
                    Placement = 1,
                    TweetVolume24 = 100
                },
                new Twitterdash.Trend{
                    TrendType = Twitterdash.TrendType.Hashtag,
                    Name = "TestTrend2",
                    Country = 23424977,
                    Placement = 1,
                    TweetVolume24 = 100
                },
                new Twitterdash.Trend
                {
                    TrendType = Twitterdash.TrendType.Hashtag,
                    Name = "TestTrend3",
                    Country = 23424977,
                    Placement = 1,
                    TweetVolume24 = 100
                }
                }
            };

            var response = await service.StoreTrends(
                request, TestServerCallContext.Create());

            var db = trendCollection.Find(new BsonDocument()).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(db.Count, Is.EqualTo(2));
                Assert.That(db[0].Country, Is.EqualTo(23424829));
                Assert.That(db[1].Country, Is.EqualTo(23424977));
            });

        }

        [Test]
        public async Task StoreSentiment_Should_Write_Sentiment_To_Database()
        {
            var service = new DatabaseWriterController(trendRepository, sentimentRepository, WOEID);

            List<SentimentPayload> sentiments = new();
            for (int i = 0; i < 5; i++)
            {
                SentimentPayload sentiment = new();
                sentiment.Sentiment = 0.5f;
                sentiment.Topic = "foobar";
                sentiment.Tweet = new()
                {
                    ID = i,
                    Timestamp = DateTime.Now.ToUniversalTime().ToTimestamp(),
                };
                sentiments.Add(sentiment);
            }


            var request = new Twitterdash.StoreSentimentRequest();
            request.Sentiments.Add(sentiments);


            var response = await service.StoreSentiment(
                request, TestServerCallContext.Create());

            var db = sentimentCollection.Find(new BsonDocument()).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(db.Count, Is.EqualTo(5));
                for (int i = 0; i < 5; i++)
                {
                    Assert.That(db[i].Tweet_ID, Is.EqualTo(i));
                }
            });
        }
    }
}