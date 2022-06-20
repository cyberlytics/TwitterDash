using conductor.activities;
using conductor.tests.Mocks;
using Elsa.ActivityResults;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace conductor.tests.activities
{
    public class PerformSentimentAnalysisTests : ActivityTestBase
    {
        private GrpcClientMocks clientMocks;
        private ILogger<PerformSentimentAnalysis> logger;

        public override void Setup()
        {
            base.Setup();
            clientMocks = new GrpcClientMocks();
            logger = BuildLogger<PerformSentimentAnalysis>();
        }

        [Test]
        public async Task PerformSentimentAnalysis_Performs_Sentiment_Analysis()
        {
            var activity = new PerformSentimentAnalysis(clientMocks.MockSentimentProviderClient(), logger);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.CleanedTweets, DataMocks.BuildTrendTweetMap(TestDataDirectory));

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });

            var sentiments = context.WorkflowInstance.Variables.Get<Dictionary<long, float>>(Nameservice.VariableNames.Sentiments);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());

                Assert.IsNotEmpty(sentiments);
            });
        }

        [Test]
        public async Task PerformSentimentAnalysis_Does_Not_Throw()
        {
            var activity = new PerformSentimentAnalysis(clientMocks.MockSentimentProviderClient(true), logger,1);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.CleanedTweets, DataMocks.BuildTrendTweetMap(TestDataDirectory));

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });

            var sentiments = context.WorkflowInstance.Variables.Get<Dictionary<long, float>>(Nameservice.VariableNames.Sentiments);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());

                Assert.IsNotNull(sentiments);
                Assert.IsEmpty(sentiments);
            });
        }
    }
}
