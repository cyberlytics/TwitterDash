using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;

namespace conductor.activities
{
    public class PerformTopicModeling : Activity
    {
        public PerformTopicModeling()
        {

        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
