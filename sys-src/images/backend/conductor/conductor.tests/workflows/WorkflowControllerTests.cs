using conductor.background_services;
using conductor.tests.Mocks;
using conductor.workflows;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace conductor.tests.workflows
{
    public class WorkflowControllerTests : WorkflowTestBase
    {
        TrendProviderService trendProviderService;
        public override void Setup()
        {
            base.Setup();
            CreateServiceProvider();
            var mocks = new GrpcClientMocks();
            trendProviderService = new TrendProviderService(mocks.MockTrendProviderClient(), BuildLogger<TrendProviderService>());
        }

        [Test]
        public async Task WorkflowController_Can_Build_Workflow()
        {
            var workflowController = new WorkflowController(trendProviderService, ServiceProvider);


            Assert.DoesNotThrowAsync(async () =>
            {
                var scope = ServiceProvider.CreateScope();
                var (blueprint,workflow) = await workflowController.BuildWorkflow<TrendWorkflow>(scope);
                Assert.IsNotNull(workflow);
                Assert.IsNotNull(blueprint);
                Assert.IsNotEmpty(workflowController.RunsInMemory);
            });
        }

        [Test]
        public async Task WorkflowController_Build_Workflow_Cleans_Up_Old_Runs()
        {
            var workflowController = new WorkflowController(trendProviderService, ServiceProvider);


            Assert.DoesNotThrowAsync(async () =>
            {
                for(int i = 0; i<20; i++)
                {
                    var scope = ServiceProvider.CreateScope();
                    var (blueprint, workflow) = await workflowController.BuildWorkflow<TrendWorkflow>(scope);
                }
            });

            Assert.AreEqual(10, workflowController.RunsInMemory.Count);
        }

        [Test]
        public async Task WorkflowController_Can_Start_Trend_Workflow()
        {
            var workflowController = new WorkflowController(trendProviderService, ServiceProvider);


            Assert.DoesNotThrowAsync(async () =>
            {
                await workflowController.StartTrendWorkflow(DataMocks.LoadTrends(TestDataDirectory));
            });
        }
    }
}
