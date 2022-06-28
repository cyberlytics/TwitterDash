using conductor.background_services;
using conductor.Nameservice;
using Elsa;
using Elsa.Builders;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.workflows
{
    public class WorkflowController
    {
        private readonly TrendProviderService trendProviderService;
        private readonly IServiceProvider services;

        public Queue<string> RunsInMemory = new();
        public WorkflowController(TrendProviderService trendProviderService, IServiceProvider services)
        {
            this.trendProviderService=trendProviderService;
            this.services=services;
            this.trendProviderService.OnTrendsRecieved += TrendProviderService_OnTrendsRecieved;
        }

        private void TrendProviderService_OnTrendsRecieved(object sender, TrendsRecievedEventArgs e)
        {
            Task.Run(()=>StartTrendWorkflow(e.Reply));
        }

        public async Task<(IWorkflowBlueprint blueprint,WorkflowInstance workflow)> BuildWorkflow<TWorkflow>(IServiceScope scope) where TWorkflow : class,IWorkflow
        {
            var workflowRegistry = scope.ServiceProvider.GetRequiredService<IWorkflowRegistry>();
            
            var workflowFactory = scope.ServiceProvider.GetRequiredService<IWorkflowFactory>();

            var workflowBlueprint = (await workflowRegistry.GetWorkflowAsync<TWorkflow>())!;

            var workflow = await workflowFactory.InstantiateAsync(workflowBlueprint);

            RunsInMemory.Enqueue(workflow.Id);
            if (RunsInMemory.Count > 10)
            {
                var instanceDeleter = scope.ServiceProvider.GetRequiredService<IWorkflowInstanceDeleter>();
                var run_to_delete = RunsInMemory.Dequeue();
                await instanceDeleter.DeleteAsync(run_to_delete.ToString());
            }
            return (workflowBlueprint,workflow);
        }

        public async Task StartTrendWorkflow(TrendProviderReply trendProviderReply){
            using var scope = services.CreateScope();

            var workflowStarter = scope.ServiceProvider.GetRequiredService<IWorkflowRunner>();
            var (workflowBlueprint, workflow) = await BuildWorkflow<TrendWorkflow>(scope);

            workflow.Variables.Set(VariableNames.Trends, trendProviderReply);
            await workflowStarter.RunWorkflowAsync(workflowBlueprint, workflow);
            GC.Collect();
        }
    }
}
