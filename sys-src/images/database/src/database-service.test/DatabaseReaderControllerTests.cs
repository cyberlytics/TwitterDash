using DatabaseService.Controller;
using DatabaseService.Tests;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace database_service.test
{
    public class DatabaseReaderControllerTests : ControllerTestBase
    {

        [Test]
        public async Task GetUniqueTweets_Should_Return_Unique_Ids()
        {
            var service = new DatabaseReaderController(trendRepository, sentimentRepository, WOEID);

            sentimentCollection.InsertOne(new()
            {
                Tweet_ID=1
            });
            sentimentCollection.InsertOne(new()
            {
                Tweet_ID=2
            });
            sentimentCollection.InsertOne(new()
            {
                Tweet_ID=5
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
