using MongoDB.Driver;
using MongoDB.Bson;
using DatabaseService.Models;
using DatabaseService.Repositories;
using Mongo2Go;


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

        [TestCase("test", 0, 2)]
        [TestCase("test", 1, 1)]
        [TestCase("bdcc", 0, 1)]
        public async Task GetAvailableSentimentTrends_Should_Return_Correct_Sentiments(string query, int limit, int expectedCount)
        {
            var sentiments = new List<Sentiment>() {
                new Sentiment { Trend = "test" },
                new Sentiment { Trend = "bdcc" },
                new Sentiment { Trend = "test" },
            };
            collection.InsertMany(sentiments);

            var actual = await repository.GetAvailableSentimentTrends(query, limit);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Exactly(expectedCount).Items);
                Assert.That(actual, Has.All.Property("Trend").EqualTo(query));
            });
        }

        [TestCase("test", 0.7f)]
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
                    Timestamp = new DateTime(2022, 1, 3, 0, 0, 0).ToUniversalTime(),
                    Value = 0.7f,
                },
            };
            collection.InsertMany(sentiments);

            var actual = await repository.GetCurrentSentiment(trendName);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.TypeOf<Sentiment>());
                Assert.That(actual, Has.Property("Trend").EqualTo(trendName));
                Assert.That(actual, Has.Property("Value").EqualTo(value));
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

            Assert.That(actual, Is.Null);
        }

        [TestCase("test", 2)]
        [TestCase("bdcc", 1)]
        public async Task GetRecentSentiment_Should_Return_Correct_Sentiments(string query, int expectedCount)
        {
            var sentiments = new List<Sentiment>() {
                new Sentiment { Trend = "test" },
                new Sentiment { Trend = "bdcc" },
                new Sentiment { Trend = "test" },
            };
            collection.InsertMany(sentiments);

            var actual = await repository.GetRecentSentiment(query, null, null);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Exactly(expectedCount).Items);
                Assert.That(actual, Has.All.Property("Trend").EqualTo(query));
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

            var actual = await repository.GetRecentSentiment("invalid", null, null);

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
                    Timestamp = new DateTime(2022, 1, 1, 0, 0, 0).ToUniversalTime(),
                },
                new Sentiment
                {
                    Trend = "bdcc",
                    Timestamp = new DateTime(2022, 1, 2, 0, 0, 0).ToUniversalTime(),
                },
                new Sentiment
                {
                    Trend = "test",
                    Timestamp = new DateTime(2022, 1, 5, 0, 0, 0).ToUniversalTime(),
                },
                new Sentiment
                {
                    Trend = "test",
                    Timestamp = new DateTime(2022, 1, 7, 0, 0, 0).ToUniversalTime(),
                },
            };
            collection.InsertMany(sentiments);

            var trendName = "test";

            var actual = await repository.GetRecentSentiment(trendName, start, end);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Has.Exactly(2).Items);
                Assert.That(actual, Has.All.Property("Trend").EqualTo(trendName));
            });
        }
    }
}