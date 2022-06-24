using MongoDB.Driver;
using MongoDB.Bson;
using DatabaseService.Models;
using DatabaseService.Repositories;
using Mongo2Go;
using DatabaseService.Controller;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Helpers;
using Twitterdash;

namespace DatabaseService.Tests
{
    [TestFixture]
    public class ControllerTestBase
    {
        protected MongoDbRunner runner;
        protected IMongoCollection<TwitterTrends> trendCollection;
        protected IMongoCollection<Sentiment> sentimentCollection;
        protected TwitterTrendsRepository trendRepository;
        protected SentimentRepository sentimentRepository;
        protected TextWriter consoleOut;
        protected woeid WOEID;


        [SetUp]
        public virtual void Setup()
        {
            var sw = new StringWriter();
            consoleOut = Console.Out;
            Console.SetOut(sw);

            runner = MongoDbRunner.Start();
            MongoClient client = new MongoClient(runner.ConnectionString);
            IMongoDatabase database = client.GetDatabase("TwitterDashTest");
            trendCollection = database.GetCollection<TwitterTrends>("TrendsTest");
            sentimentCollection = database.GetCollection<Sentiment>("SentimentTest");

            trendRepository = new TwitterTrendsRepository(trendCollection);
            sentimentRepository = new SentimentRepository(sentimentCollection);
            WOEID = new woeid();
        }

        [TearDown]
        public virtual void Teardown()
        {
            runner.Dispose();
            Console.SetOut(consoleOut);
        }

    }
}