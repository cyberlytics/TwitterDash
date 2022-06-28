using DatabaseService.Models;
using DatabaseService.Repositories;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using Twitterdash;

namespace DatabaseService.Tests
{
    [TestFixture]
    class sentimentRepositoryTest
    {
        private MongoDbRunner runner;
        private IMongoCollection<Sentiment> collection;
        private SentimentRepository repository;
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
            collection = database.GetCollection<Sentiment>("SentimentTest");

            repository = new SentimentRepository(collection);
        }

        [TearDown]
        public void Teardown()
        {
            runner.Dispose();
            Console.SetOut(consoleOut);
        }

        [Test]
        public async Task StoreSentiment_Should_Insert()
        {
            var expected = new List<Sentiment>
            {
                new Sentiment
                {
                    ID = "abcdabcdabcdabcdabcdabcd",
                    Tweet_ID = 1234567890,
                    Timestamp = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime(),
                    Trend = "TestTrend",
                    Value = 0.7f,
                }
            };

            await repository.StoreSentiment(expected);

            var result = collection.FindSync(Builders<Sentiment>.Filter.Empty).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Exactly(1).Items);
                Assert.That(result.ToJson(), Is.EqualTo(expected.ToJson()));
            });
        }

        [TestCase("test", 0, 1)]
        [TestCase("test", 1, 1)]
        [TestCase("bdcc", 0, 1)]
        [TestCase("bd", 0, 1)]
        public async Task GetTrendsWithAvailableSentiment_Should_Return_Correct_Trends(string query, int limit, int expectedCount)
        {
            var sentiments = new List<Sentiment>() {
                new Sentiment { Trend = "test" },
                new Sentiment { Trend = "bdcc" },
                new Sentiment { Trend = "test" },
            };
            collection.InsertMany(sentiments);

            var actual = await repository.GetTrendsWithAvailableSentiment(query, limit);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Exactly(expectedCount).Items);
                Assert.That(actual, Has.All.Contains(query));
            });
        }

        [TestCase("test", 0.2f)]
        [TestCase("bdcc", 1.0f)]
        public async Task GetCurrentSentiment_Should_Return_Correct_Sentiment(string trendName, float value)
        {
            var sentiments = new List<Sentiment>() {
                new Sentiment
                {
                    Trend = "test",
                    Timestamp = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime(),
                    Value = 0.5f,
                },
                new Sentiment
                {
                    Trend = "bdcc",
                    Timestamp = new DateTime(2022, 1, 2, 0, 0, 0).ToUniversalTime(),
                    Value = 1.0f,
                },
                new Sentiment
                {
                    Trend = "test",
                    Timestamp = new DateTime(2022, 1, 3, 23, 0, 0).ToUniversalTime(),
                    Value = 0.6f,
                },
                new Sentiment
                {
                    Trend = "test",
                    Timestamp = new DateTime(2022, 1, 3, 23, 15, 0).ToUniversalTime(),
                    Value = -1f,
                },
                new Sentiment
                {
                    Trend = "test",
                    Timestamp = new DateTime(2022, 1, 3, 23, 30, 0).ToUniversalTime(),
                    Value = 1f,
                },
            };
            collection.InsertMany(sentiments);

            var actual = await repository.GetCurrentSentiment(trendName);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(value,actual);
            });
        }

        [Test]
        public async Task GetCurrentSentiment_Should_Return_No_Sentiment()
        {
            var sentiments = new List<Sentiment>() {
                new Sentiment
                {
                    Trend = "test",
                    Timestamp = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime(),
                    Value = 0.5f,
                },
                new Sentiment
                {
                    Trend = "bdcc",
                    Timestamp = new DateTime(2022, 1, 2, 0, 0, 0).ToUniversalTime(),
                    Value = 1.0f,
                },
            };
            collection.InsertMany(sentiments);

            var actual = await repository.GetCurrentSentiment(null);

            Assert.AreEqual(0.0f, actual);
        }

        [TestCase("test", 1, 0.5f)]
        [TestCase("bdcc", 1, 1)]
        public async Task GetRecentSentiment_Should_Return_Correct_Sentiments(string query, int expectedCount, float expectedMean)
        {
            var sentiments = new List<Sentiment>() {
                new Sentiment { Trend = "test" , Value = 1,Timestamp=DateTime.Now},
                new Sentiment { Trend = "bdcc" , Value = 1,Timestamp=DateTime.Now},
                new Sentiment { Trend = "test" , Value = 0,Timestamp=DateTime.Now},
            };
            collection.InsertMany(sentiments);

            var actual = await repository.GetRecentSentiments(query, null, null, Granularity.Hour);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Exactly(expectedCount).Items);
                if (expectedCount > 0)
                    Assert.That(actual.First().Mean, Is.EqualTo(expectedMean));
            });
        }

        [Test]
        public async Task GetRecentSentiment_Should_Return_No_Sentiments()
        {
            var sentiments = new List<Sentiment>() {
                new Sentiment { Trend = "test" },
                new Sentiment { Trend = "bdcc" },
                new Sentiment { Trend = "test" },
            };
            collection.InsertMany(sentiments);

            var actual = await repository.GetRecentSentiments("invalid", null, null, Granularity.Hour);

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public async Task GetRecentSentiment_Within_Timeframe_Should_Return_Sentiments_Of_That_Timeframe()
        {
            var start = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime();
            var end = new DateTime(2022, 1, 6, 0, 0, 0).ToUniversalTime();

            var sentiments = new List<Sentiment>() {
                new Sentiment
                {
                    Trend = "test",
                    Value = 1,
                    Timestamp = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime(),
                },
                new Sentiment
                {
                    Trend = "bdcc",
                    Value = -1,
                    Timestamp = new DateTime(2022, 1, 2, 0, 0, 0).ToUniversalTime(),
                },
                new Sentiment
                {
                    Trend = "test",
                    Value = 1,
                    Timestamp = new DateTime(2022, 1, 5, 0, 0, 0).ToUniversalTime(),
                },
                new Sentiment
                {
                    Trend = "test",
                    Value = 1,
                    Timestamp = new DateTime(2022, 1, 7, 0, 0, 0).ToUniversalTime(),
                },
            };
            collection.InsertMany(sentiments);

            var trendName = "test";

            var actual = await repository.GetRecentSentiments(trendName, start, end, Granularity.Day);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Exactly(2).Items);
                Assert.That(actual, Has.All.Property("Mean").EqualTo(1));
            });
        }

        [Test]
        public async Task GetRecentSentiment_Returns_Correct_Batches ()
        {
            var start = new DateTime(2022, 1, 2, 0, 0, 0).ToUniversalTime();
            var end = new DateTime(2022, 1, 31, 0, 0, 0).ToUniversalTime();


            var sentiments = new List<Sentiment>();
            var startTimes = new List<DateTime>();
            for (int i = 0; i<30; i++)
            {
                startTimes.Add(new DateTime(2022, 1, 2+i, 0, 0, 0).ToUniversalTime());
                for (int j=0;j<24;j++)
                    sentiments.Add(new()
                    {
                        Trend = "test",
                        Value = i+1,
                        Timestamp = new DateTime(2022, 1, 2+i, j, 0, 0).ToUniversalTime(),
                    });
            }

            collection.InsertMany(sentiments);

            var trendName = "test";

            var actual = await repository.GetRecentSentiments(trendName, start, end, Granularity.Day);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Exactly(30).Items);
                foreach (var batch in actual)
                    Assert.That(batch.Time.Day, Is.EqualTo(batch.Mean));
            });
        }
    }
}