using conductor.activities;
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
                .StartWith<ValidateTrends>()
                .Then<PersistTrends>();
        }
    }
}
