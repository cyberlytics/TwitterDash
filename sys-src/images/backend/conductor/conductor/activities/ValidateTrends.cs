using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{
    public class ValidateTrends : Activity
    {
        public ValidateTrends()
        {
            this.DisplayName = "Validate the recieved Trends";
        }


        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var trends = context.WorkflowInstance.Variables.Get("Trends") as TrendProviderReply;

            return Outcome(trends != null ? Nameservice.Outcomes.Success : Nameservice.Outcomes.Failure);
        }
    }
}
