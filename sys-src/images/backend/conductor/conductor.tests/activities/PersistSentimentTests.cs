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
    public class PersistSentimentTests : ActivityTestBase
    {
        private GrpcClientMocks clientMocks;
        private ILogger<PersistSentiment> logger;

        public override void Setup()
        {
            base.Setup();
            clientMocks = new GrpcClientMocks();
            logger = BuildLogger<PersistSentiment>();
        }

        [Test]
        public async Task PersistSentiment_Should_Persist_The_Sentiment()
        {
            var activity = new PersistSentiment(clientMocks.MockDatabaseWriterClient(), logger);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.CleanedTweets, DataMocks.BuildTrendTweetMap(TestDataDirectory));
            var sentiments = DataMocks.BuildSentiment(DataMocks.BuildTrendTweetMap(TestDataDirectory));
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.Sentiments, sentiments);

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });


            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());

                Assert.IsNotEmpty(clientMocks.DatabaseWriterClient_StoreSentimentAsync_Calls);
                Assert.AreEqual(sentiments.Count, clientMocks.DatabaseWriterClient_StoreSentimentAsync_Calls.First().Sentiments.Count);
            });
        }

        [Test]
        public async Task PersistSentiment_Should_Not_Throw()
        {
            var activity = new PersistSentiment(clientMocks.MockDatabaseWriterClient(true), logger);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.CleanedTweets, DataMocks.BuildTrendTweetMap(TestDataDirectory));
            var sentiments = DataMocks.BuildSentiment(DataMocks.BuildTrendTweetMap(TestDataDirectory));
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.Sentiments, sentiments);

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });


            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());

                Assert.IsEmpty(clientMocks.DatabaseWriterClient_StoreSentimentAsync_Calls);
            });
        }

        [Test]
        public async Task PersistSentiment_Works_With_Empty_Sentiment()
        {
            var activity = new PersistSentiment(clientMocks.MockDatabaseWriterClient(true), logger);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.CleanedTweets, DataMocks.BuildTrendTweetMap(TestDataDirectory));
            var sentiments = new Dictionary<long, float>();
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.Sentiments, sentiments);

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });


            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());

                Assert.IsEmpty(clientMocks.DatabaseWriterClient_StoreSentimentAsync_Calls);
            });
        }
    }
}
