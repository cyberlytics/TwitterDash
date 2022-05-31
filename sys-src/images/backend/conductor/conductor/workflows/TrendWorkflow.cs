using conductor.activities;
using Elsa.Activities.ControlFlow;
using Elsa.Builders;

namespace conductor.workflows
{
    public class TrendWorkflow : IWorkflow
    {
        public TrendWorkflow()
        {

        }

        public void Build(IWorkflowBuilder builder)
        {
            builder
                .WithDisplayName("Trend Workflow")
                .Then<ValidateTrends>()
                //.Then<CollectTweets>()
                .Then<PersistTrends>();
        }
    }
}
