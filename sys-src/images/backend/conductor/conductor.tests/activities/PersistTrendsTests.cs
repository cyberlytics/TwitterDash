using conductor.activities;
using conductor.tests.Mocks;
using Elsa.ActivityResults;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Twitterdash;

namespace conductor.tests.activities
{
    public class PersistTrendsTests : ActivityTestBase
    {
        GrpcClientMocks clientMocks;
        ILogger<PersistTrends> logger;
        public override void Setup()
        {
            base.Setup();
            clientMocks = new GrpcClientMocks();
            logger = BuildLogger<PersistTrends>();
        }

        [Test]
        public async Task PersistTrends_Should_Send_Trends_To_Database()
        {
            var activity = new PersistTrends(clientMocks.MockDatabaseWriterClient(),logger);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.Trends, DataMocks.LoadTrends(TestDataDirectory));

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.IsNotEmpty(result.Outcomes);
                Assert.IsNotEmpty(LogMessages);
                Assert.AreEqual("Stored 54 Trends in Database!", LogMessages[0].Message);
                Assert.IsNotEmpty(clientMocks.DatabaseWriterClient_StoreTrendsAsync_Calls);
                Assert.AreEqual(Nameservice.Outcomes.Done, result.Outcomes.First());
            });
        }

        [Test]
        public async Task PersistTrends_Should_Fail_When_Client_Throws()
        {
            var activity = new PersistTrends(clientMocks.MockDatabaseWriterClient(true), logger);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.Trends, DataMocks.LoadTrends(TestDataDirectory));

            OutcomeResult result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                result = (OutcomeResult)await activity.ExecuteAsync(context);
            });

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.IsNotEmpty(LogMessages);
                StringAssert.Contains("Failed to Persist Trends with Error:", LogMessages[0].Message);
                Assert.IsEmpty(clientMocks.DatabaseWriterClient_StoreTrendsAsync_Calls);
            });
        }
    }
}
