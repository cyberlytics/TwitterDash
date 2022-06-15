using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{
    public class MakeTweetsUnique : Activity
    {


        public MakeTweetsUnique()
        {
            
        }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var gatheredTweets = context.WorkflowInstance.Variables.Get<Dictionary<string, List<Tweet>>>(Nameservice.VariableNames.GatheredTweets)!;

            //TODO
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.CleanedTweets, gatheredTweets);
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
