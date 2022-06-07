using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;

namespace conductor.activities
{
    public class MakeTweetsUnique : Activity
    {


        public MakeTweetsUnique()
        {
            
        }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
