
using conductor.Nameservice;
using conductor.workflows;
using Elsa.Models;
using Elsa.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Twitterdash;


namespace conductor.tests.workflows
{
    class TrendWorkflowTests : WorkflowTestBase
    {
        List<TrendProviderReply> TrendProviderReply_Send_to_Database = new();

        private TrendProviderReply LoadTrends()
        {
            var trendsjson = Path.Combine(TestDataDirectory, "trends-example.json");
            var jsonDoc = JsonDocument.Parse(File.ReadAllText(trendsjson));
            List<Twitterdash.Trend> trends = new();

            foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
            {
                trends.Add(new Twitterdash.Trend()
                {
                    Country = jsonElement.GetProperty("country").GetInt32(),
                    Name = jsonElement.GetProperty("hashtag").GetString(),
                    Placement =  jsonElement.GetProperty("top").GetInt32(),
                    TrendType = Twitterdash.TrendType.Hashtag,
                    TweetVolume24 = jsonElement.GetProperty("tweet_volume").GetInt32()
                });
            }

            var trendReply = new TrendProviderReply();
            trendReply.Trends.AddRange(trends);
            trendReply.Timestamp = new(DateTime.Now.ToUniversalTime().ToTimestamp());
            return trendReply;
        }

        private DatabaseWriter.DatabaseWriterClient MockDatabaseWriterClient(bool throws=false)
        {
            var DatabaseWriter_mock = new Mock<DatabaseWriter.DatabaseWriterClient>();

            var mocked_call = TestCalls.AsyncUnaryCall<Empty>(
               Task.FromResult(new Empty()),
               Task.FromResult(new Metadata()),
               () => Status.DefaultSuccess,
               () => new Metadata(),
               () => { });

            DatabaseWriter_mock.Setup(g => g.StoreTrendsAsync(
                It.IsAny<TrendProviderReply>(),
                null,
                null,
                CancellationToken.None))
                .Returns(mocked_call)
                .Callback<TrendProviderReply, object, object, object>
                ((TrendProviderReply, Metadata, DateTime, CancellationToken) =>
                {
                    TrendProviderReply_Send_to_Database.Add(TrendProviderReply);
                });

            return DatabaseWriter_mock.Object;
        }

        [Test]
        public async Task TrendWorkflow_Should_Run_With_Valid_Data()
        {
            builder.Services.AddSingleton(MockDatabaseWriterClient());

            CreateServiceProvider();
            using var scope = ServiceProvider.CreateScope();
            var (workflowBlueprint, workflow) = await BuildWorkflow<TrendWorkflow>(scope);


            workflow.Variables.Set(VariableNames.Trends, LoadTrends());
            var workflowStarter = scope.ServiceProvider.GetRequiredService<IWorkflowRunner>();

            var result = await workflowStarter.RunWorkflowAsync(workflowBlueprint, workflow);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(result.Executed);
                Assert.AreEqual(WorkflowStatus.Finished, result.WorkflowInstance.WorkflowStatus);
                Assert.IsNull(result.WorkflowInstance.Fault);
            });
        }
    }
}
