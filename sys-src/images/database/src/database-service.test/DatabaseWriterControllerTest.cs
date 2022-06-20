using MongoDB.Driver;
using MongoDB.Bson;
using DatabaseService.Models;
using DatabaseService.Repositories;
using Mongo2Go;
using DatabaseService.Controller;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Helpers;

namespace DatabaseService.Tests
{
    [TestFixture]
    class DatabaseWriterControllerTest
    {
        private MongoDbRunner runner;
        private IMongoCollection<TwitterTrends> collection;
        private TwitterTrendsRepository repository;
        private TextWriter consoleOut;
        private woeid WOEID;
        
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
            
            WOEID = new woeid();          
        }

        [TearDown]
        public void Teardown()
        {
            runner.Dispose();
            Console.SetOut(consoleOut);
        }

        [Test]
        public async Task StoreTrends_Should_Write_Trends_To_Database()
        {
            var service = new DatabaseWriterController(repository, WOEID);

            var request = new Twitterdash.TrendProviderReply(){
                Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                Trends = { new Twitterdash.Trend{
                    TrendType = Twitterdash.TrendType.Hashtag,
                    Name = "TestTrend",
                    Country = 23424829,
                    Placement = 1,
                    TweetVolume24 = 100
                }}
            };

            var response = await service.StoreTrends(
                request, TestServerCallContext.Create());

            var db = collection.Find(new BsonDocument()).ToList();
            Assert.That(db.Count, Is.EqualTo(1));
            Assert.That(db[0].Trends[0].name, Is.EqualTo("TestTrend"));
        }
    }
}