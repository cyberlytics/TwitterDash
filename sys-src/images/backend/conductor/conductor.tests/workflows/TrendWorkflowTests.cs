
using conductor.Nameservice;
using conductor.tests.activities;
using conductor.tests.Mocks;
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
        GrpcClientMocks clientMocks;
        public override void Setup()
        {
            base.Setup();
            clientMocks = new GrpcClientMocks();
        }

        [Test]
        public async Task TrendWorkflow_Should_Run_With_Valid_Data()
        {
            builder.Services.AddSingleton(clientMocks.MockDatabaseWriterClient());
            builder.Services.AddSingleton(clientMocks.MockTweetProviderClient(TestDataDirectory));
            builder.Services.AddSingleton(clientMocks.MockSentimentProviderClient());

            
            CreateServiceProvider();
            using var scope = ServiceProvider.CreateScope();
            var (workflowBlueprint, workflow) = await BuildWorkflow<TrendWorkflow>(scope);


            workflow.Variables.Set(VariableNames.Trends, DataMocks.LoadTrends(TestDataDirectory));
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
