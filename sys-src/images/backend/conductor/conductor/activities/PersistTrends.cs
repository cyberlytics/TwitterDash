using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{

    public class PersistTrends : Activity
    {
        private readonly DatabaseWriter.DatabaseWriterClient client;

        public PersistTrends(DatabaseWriter.DatabaseWriterClient client)
        {
            this.DisplayName = "Persist Trends";
            this.Description = "Persiste Trends to the Database-Service.";
            this.client=client;
        }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var trends = (TrendProviderReply)context.WorkflowInstance.Variables.Get("Trends")!;
            try
            {
                await client.StoreTrendsAsync(trends);
            }
            catch (Exception ex)
            {
                return Fault(ex.ToString());
            }
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
