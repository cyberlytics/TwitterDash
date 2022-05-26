using conductor.background_services;
using conductor.Nameservice;
using Elsa;
using Elsa.Services;
using Twitterdash;

namespace conductor.workflows
{
    public class WorkflowController
    {
        private readonly TrendProviderService trendProviderService;
        private readonly IServiceProvider services;

        public int Runs { get; private set; } = 0;
        private Queue<string> RunsInMemory = new();
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

        public async void StartTrendWorkflow(TrendProviderReply trendProviderReply){
            using var scope = services.CreateScope();

            Runs++;

            var workflowRegistry = scope.ServiceProvider.GetRequiredService<IWorkflowRegistry>();
            var workflowStarter= scope.ServiceProvider.GetRequiredService<IWorkflowRunner>();
            var workflowFactory = scope.ServiceProvider.GetRequiredService<IWorkflowFactory>();

            
            
            var workflowBlueprint = (await workflowRegistry.GetWorkflowAsync<TrendWorkflow>())!;

            var workflow = await workflowFactory.InstantiateAsync(workflowBlueprint,correlationId: Runs.ToString(),contextId: Runs.ToString());
            

            RunsInMemory.Enqueue(workflow.Id);
            if (RunsInMemory.Count > 10)
            {
                var instanceDeleter = scope.ServiceProvider.GetRequiredService<IWorkflowInstanceDeleter>();
                var run_to_delete = RunsInMemory.Dequeue();
                await instanceDeleter.DeleteAsync(run_to_delete.ToString());
            }

            workflow.Variables.Set(VariableNames.Trends, trendProviderReply);
            await workflowStarter.RunWorkflowAsync(workflowBlueprint, workflow);
            GC.Collect();
        }
    }
}
