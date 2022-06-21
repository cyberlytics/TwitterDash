using DatabaseService.Controller;
using DatabaseService.Tests;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using DatabaseService.Models;
using DatabaseService.Repositories;
using Mongo2Go;
using Google.Protobuf.WellKnownTypes;


namespace DatabaseService.Tests
{
    public class DatabaseReaderControllerTests : ControllerTestBase
    {
        private List<TwitterTrends> TestTrends = new List<TwitterTrends>
        {   new TwitterTrends{
                DateTime = new DateTime(2021, 1, 1, 0, 0, 0).ToUniversalTime(),
                Country = 23424900,
                Trends = new List<TwitterTrend> {
                    new TwitterTrend {
                        trendType = TrendType.Hashtag,
                        name = "#test1",
                        woeid = 23424900,
                        placement = 1,
                        tweetVolume24 = 10
                    },
                    new TwitterTrend {
                        trendType = TrendType.Hashtag,
                        name = "#test2",
                        woeid = 23424900,
                        placement = 2,
                        tweetVolume24 = 5
                    }
                }
            },
            new TwitterTrends{
                DateTime = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime(),
                Country = 23424900,
                Trends = new List<TwitterTrend> {
                    new TwitterTrend {
                        trendType = TrendType.Hashtag,
                        name = "#test1",
                        woeid = 23424900,
                        placement = 2,
                        tweetVolume24 = 10
                    },
                    new TwitterTrend {
                        trendType = TrendType.Hashtag,
                        name = "#test3",
                        woeid = 23424900,
                        placement = 1,
                        tweetVolume24 = 5
                    }
                }
            },
            new TwitterTrends{
                DateTime = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime(),
                Country = 23424768,
                Trends = new List<TwitterTrend> {
                    new TwitterTrend {
                        trendType = TrendType.Hashtag,
                        name = "#test5",
                        woeid = 23424768,
                        placement = 1,
                        tweetVolume24 = 10
                    },
                    new TwitterTrend {
                        trendType = TrendType.Hashtag,
                        name = "#test6",
                        woeid = 23424768,
                        placement = 2,
                        tweetVolume24 = 5
                    }
                }
            },
        };

        [Test]
        public async Task GetAvailableCountries_Should_Return_Correct_Country_Codes()
        {
            trendCollection.InsertMany(TestTrends);
            var service = new DatabaseReaderController(trendRepository, sentimentRepository, WOEID);

            var request = new Empty();

            var response = await service.GetAvailableCountries(
                request, TestServerCallContext.Create());

            Assert.Multiple(() =>
            {
                Assert.That(response.Countries.Count, Is.EqualTo(2));
                Assert.That(response.Countries.ToList(), Is.EqualTo(new List<string> { "Brazil", "Mexico" }));
            }); 
        }

        [TestCase(0, 2)]
        [TestCase(1, 1)]

        public async Task GetCurrentTrends_Should_Return_Current_Trends(int Limit, int count)
        {
            var service = new DatabaseReaderController(trendRepository, sentimentRepository, WOEID);
            var request = new Twitterdash.GetCurrentTrendsRequest
            {
                Country = "Brazil",
                Limit = Limit
            };

            var response1 = await service.GetCurrentTrends(
                request, TestServerCallContext.Create());

            trendCollection.InsertMany(TestTrends);         

            var response2 = await service.GetCurrentTrends(
                request, TestServerCallContext.Create());

            Assert.Multiple(() =>
            {
                Assert.That(response1.Trends.Count, Is.EqualTo(0));
                Assert.That(response2.Trends.Count, Is.EqualTo(count));
                Assert.That(response2.Trends[0].Country, Is.EqualTo(WOEID.getWOEID("Brazil")));
            });
            
        }

        [TestCase("Brazil", "#test1", 2)]
        [TestCase("Brazil", "#test2", 1)]
        [TestCase("Brazil", "#testNope", 0)]
        [TestCase("Lummerland", "#Jim Knopf", 0)]
        public async Task GetRecentTrends_Should_Return_Recent_Trends(string Country, string Hastag, int count)
        {
            trendCollection.InsertMany(TestTrends);
            var service = new DatabaseReaderController(trendRepository, sentimentRepository, WOEID);

            var request = new Twitterdash.GetRecentTrendsRequest
            {
                Country = Country,
                Hashtag = Hastag
            };

            var response = await service.GetRecentTrends(
                request, TestServerCallContext.Create());

            Assert.Multiple(() =>
            {
                Assert.That(response.RecentTrends.Count, Is.EqualTo(count));
            });
        }

        [TestCase("01/02/2020", "01/02/2022", 2)]
        [TestCase("10/10/2021", "01/02/2022", 1)]

        public async Task GetRecentTrends_of_Timeframe_Should_Return_Recent_Trends(string startDate, string endDate, int count)
        {
            trendCollection.InsertMany(TestTrends);
            var service = new DatabaseReaderController(trendRepository, sentimentRepository, WOEID);
            var request = new Twitterdash.GetRecentTrendsRequest
            {
                Country = "Brazil",
                Hashtag = "#test1",
                StartDate = Timestamp.FromDateTime(Convert.ToDateTime(startDate).ToUniversalTime()),
                EndDate = Timestamp.FromDateTime(Convert.ToDateTime(endDate).ToUniversalTime()),
            };

            var response = await service.GetRecentTrends(
                request, TestServerCallContext.Create());

            Assert.Multiple(() =>
            {
                Assert.That(response.RecentTrends.Count, Is.EqualTo(count));
            });
        }

        [Test]
        public async Task GetUniqueTweets_Should_Return_Unique_Ids()
        {
            var service = new DatabaseReaderController(trendRepository, sentimentRepository, WOEID);

            sentimentCollection.InsertOne(new()
            {
                Tweet_ID = 1
            });
            sentimentCollection.InsertOne(new()
            {
                Tweet_ID = 2
            });
            sentimentCollection.InsertOne(new()
            {
                Tweet_ID = 5
            });


            var query_ids = new List<long>() { 1, 2, 3, 4, 5 };

            var request = new Twitterdash.GetUniqueTweetsPayload();
            request.TweetIds.Add(query_ids);


            var response = await service.GetUniqueTweets(
                request, TestServerCallContext.Create());

            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(response.TweetIds);
                Assert.AreEqual(3, response.TweetIds[0]);
                Assert.AreEqual(4, response.TweetIds[1]);
            });
        }
    }
}

















namespace DatabaseService.Tests
{
    [TestFixture]
    class DatabaseReaderControllerTest
    {


        

        
    }
}