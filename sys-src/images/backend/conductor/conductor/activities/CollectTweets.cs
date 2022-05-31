using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;

namespace conductor.activities
{
    public class CollectTweets : Activity
    {

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
