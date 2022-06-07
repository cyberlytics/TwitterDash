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
                .WithDescription("Validate the Trends recieved from the Trend-Service")
                .WithId(nameof(ValidateTrends))
                .WithDisplayName("Validate Trends")
                .ParallelForEach(new string[] { "Persist", "Gather Tweets" }, iterate =>
                  iterate.If(ctx => (string)ctx.Input == "Persist")
                      .Then<PersistTrends>()
                      .WithDescription("Persist the trends to a Database")
                      .WithId(nameof(PersistTrends))
                      .WithDisplayName("Save Trends")
                      .ThenNamed("Wait")


                );
                //.Then<CollectTweets>()

        }
    }
}
