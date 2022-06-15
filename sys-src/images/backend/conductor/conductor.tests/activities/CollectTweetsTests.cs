using conductor.activities;
using conductor.tests.Mocks;
using Elsa.ActivityResults;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitterdash;

namespace conductor.tests.activities
{
    public class CollectTweetsTests : ActivityTestBase
    {
        GrpcClientMocks clientMocks;
        ILogger<CollectTweets> logger;

        public override void Setup()
        {
            base.Setup();
            clientMocks = new GrpcClientMocks();
            logger = BuildLogger<CollectTweets>();
        }

        [Test]
        public async Task CollectTweets_Should_Collect_Tweets()
        {
            var activity = new CollectTweets(clientMocks.MockTweetProviderClient(TestDataDirectory), logger);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.Trends, DataMocks.LoadTrends(TestDataDirectory));

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });


            var collectedTweets = context.WorkflowInstance.Variables.Get<Dictionary<string, List<Tweet>>>(Nameservice.VariableNames.GatheredTweets);
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());

                Assert.IsNotNull(collectedTweets);
                Assert.IsNotEmpty(collectedTweets);
                Assert.AreEqual(49, collectedTweets.Count);

                Assert.IsNotEmpty(LogMessages);
                Assert.IsNotEmpty(clientMocks.TweetProvider_GetTweetsAsync_Calls);
                foreach (var call in clientMocks.TweetProvider_GetTweetsAsync_Calls)
                {
                    Assert.IsNotEmpty(call.Languages);
                    Assert.IsNotNull(call.Since);
                    Assert.IsNotNull(call.Until);
                    Assert.IsNotNull(call.Languages);
                    Assert.AreEqual(1000, call.Limit);
                }
            });
        }

        [Test]
        public async Task CollectTweets_Should_Not_Throw()
        {
            var activity = new CollectTweets(clientMocks.MockTweetProviderClient(TestDataDirectory,true), logger);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.Trends, DataMocks.LoadTrends(TestDataDirectory));

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });


            var collectedTweets = context.WorkflowInstance.Variables.Get<Dictionary<string, List<Tweet>>>(Nameservice.VariableNames.GatheredTweets);
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());

                Assert.IsNotNull(collectedTweets);
                Assert.IsEmpty(collectedTweets);

                Assert.IsNotEmpty(LogMessages);
            });
        }
    }
}
