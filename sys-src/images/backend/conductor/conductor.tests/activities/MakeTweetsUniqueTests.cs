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
    public class MakeTweetsUniqueTests : ActivityTestBase
    {
        GrpcClientMocks clientMocks;
        ILogger<MakeTweetsUnique> logger;
        Dictionary<string, List<Tweet>> trendTweetMap;
        public override void Setup()
        {
            base.Setup();
            clientMocks = new GrpcClientMocks();
            logger = BuildLogger<MakeTweetsUnique>();
            trendTweetMap = DataMocks.BuildTrendTweetMap(TestDataDirectory);
        }

        [Test]
        public async Task MakeTweetsUnique_Should_Filter_GatheredTweets()
        {
            var activity = new MakeTweetsUnique();
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.GatheredTweets, trendTweetMap);

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });


            var cleanedTweets = context.WorkflowInstance.Variables.Get<Dictionary<string, List<Tweet>>>(Nameservice.VariableNames.CleanedTweets);
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());

                Assert.IsNotNull(cleanedTweets);
                Assert.IsNotEmpty(cleanedTweets);

            });
        }
    }
}
