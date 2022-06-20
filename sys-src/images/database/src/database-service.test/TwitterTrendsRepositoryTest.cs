using MongoDB.Driver;
using MongoDB.Bson;
using DatabaseService.Models;
using DatabaseService.Repositories;
using Mongo2Go;


namespace DatabaseService.Tests
{
    [TestFixture]
    class TwitterTrendsRepositoryTests
    {
        private MongoDbRunner runner;
        private IMongoCollection<TwitterTrends> collection;
        private TwitterTrendsRepository repository;
        private TextWriter consoleOut;
        

        [SetUp]
        public void Setup()
        {
            var sw = new StringWriter();
            consoleOut = Console.Out;
            Console.SetOut(sw);

            runner = MongoDbRunner.Start();
            MongoClient client = new MongoClient(runner.ConnectionString);
            IMongoDatabase database = client.GetDatabase("TwitterDashTest");
            collection = database.GetCollection<TwitterTrends>("TrendsTest");

            repository = new TwitterTrendsRepository(collection);
        }

        [TearDown]
        public void Teardown()
        {
            runner.Dispose();
            Console.SetOut(consoleOut);
        }

        [Test]
        public async Task StoreTrends_Should_Insert()
        {
            var expected = new TwitterTrends
            {
                ID = "abcdabcdabcdabcdabcdabcd",
                DateTime = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime(),
                Country = 1,
                Trends = new List<TwitterTrend> {
                new TwitterTrend {
                    trendType = TrendType.Hashtag,
                    name = "#test1",
                    woeid = 1,
                    placement = 1,
                    tweetVolume24 = 10
                },
                new TwitterTrend {
                    trendType = TrendType.Hashtag,
                    name = "#test2",
                    woeid = 1,
                    placement = 2,
                    tweetVolume24 = 5
                }
            }
            };

            await repository.StoreTrends(expected);

            var result = collection.FindSync(Builders<TwitterTrends>.Filter.Empty).ToList();
            var actual = result.First();

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Exactly(1).Items);
                Assert.That(actual.ToJson(), Is.EqualTo(expected.ToJson()));
            });
        }

        [Test]
        public async Task GetAvailableCountries_Should_Return_Correct_Country_Codes()
        {
            var expected = new List<int>() { 1, 2, 3 };
            var trends = new List<TwitterTrends>() {
                new TwitterTrends() { Country = 1 },
                new TwitterTrends() { Country = 2 },
                new TwitterTrends() { Country = 3 },
            };
            collection.InsertMany(trends);

            var actual = await repository.GetAvailableCountries();

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Exactly(trends.Count).Items);
                Assert.That(actual, Is.EquivalentTo(expected));
            });
        }

        [TestCase(null, 3)]
        [TestCase(2, 2)]
        [TestCase(5, null)]
        public async Task GetCurrentTrends_Should_Return_Correct_Trends(int? woeid, int? expected)
        {
            var trends = new List<TwitterTrends>() {
                new TwitterTrends() {
                    Country = 1,
                    DateTime = new DateTime(2022, 1, 1, 12, 0, 0).ToUniversalTime()
                },
                new TwitterTrends() {
                    Country = 2,
                    DateTime = new DateTime(2022, 1, 1, 14, 0, 0).ToUniversalTime()
                },
                new TwitterTrends() {
                    Country = 3,
                    DateTime = new DateTime(2022, 1, 1, 16, 0, 0).ToUniversalTime()
                },
            };
            collection.InsertMany(trends);

            var actual = await repository.GetCurrentTrends(woeid);

            Assert.That(actual?.Country, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetRecentTrends_Should_Return_Empty_Trends_On_Empty_String()
        {
            var trends = new List<TwitterTrends>() {
                new TwitterTrends() {
                    Trends = new() {
                        new() { name = "#bigdata" },
                        new() { name = "#cloudcomputing" },
                    }
                },
                new TwitterTrends() {
                    Trends = new() {
                        new() { name = "#unit" },
                        new() { name = "#testing" },
                    }
                },
            };

            var actual = await repository.GetRecentTrends(null, null, "");

            Assert.That(actual, Has.Exactly(0).Items);
        }

        [Test]
        public async Task GetRecentTrends_Should_Return_Trends_Of_Given_String()
        {
            var trends = new List<TwitterTrends>() {
                new TwitterTrends() {
                    Trends = new() {
                        new() { name = "#bigdata" },
                        new() { name = "#cloudcomputing" },
                    }
                },
                new TwitterTrends()
                {
                    Trends = new() {
                            new() { name = "#unit" },
                            new() { name = "#testing" },
                        }
                }
            };

            var resName = "#testing";
            
            foreach (var trend in trends)
            {
                await repository.StoreTrends(trend);
            }

            var result = await repository.GetRecentTrends(null, null, resName);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Exactly(1).Items);
                Assert.That(result.First().Trends.First().name, Is.EqualTo(resName));
            });
        }

        [Test]
        public async Task GetRecentTrends_within_Timeframe_Should_Return_Trends_Of_Given_String()
        {
            var startTime = new DateTime(2022, 11, 2, 0, 0, 0);
            var endTime = new DateTime(2022, 12, 22, 0, 0, 0);
            
            var trends = new List<TwitterTrends>() {
                new TwitterTrends() {
                    DateTime = new DateTime(2022, 10, 20, 0, 0, 0),
                    Trends = new() {
                        new() { name = "#testing" },
                    }
                },
                new TwitterTrends()
                {
                    DateTime = new DateTime(2022, 11, 20, 0, 0, 0),
                    Trends = new() {
                            new() { name = "#testing" },
                        }
                },
                new TwitterTrends()
                {
                    DateTime = new DateTime(2022, 12, 20, 0, 0, 0),
                    Trends = new() {
                            new() { name = "#testing" },
                        }
                }                
            };

            var resName = "#testing";

            foreach (var trend in trends)
            {
                await repository.StoreTrends(trend);
            }

            var result = await repository.GetRecentTrends(startTime, endTime, resName);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Exactly(2).Items);
            });
        }
    }
}