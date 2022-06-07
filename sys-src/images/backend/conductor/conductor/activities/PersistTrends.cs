using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{

    
    public class PersistTrends : Activity
    {
        private readonly DatabaseWriter.DatabaseWriterClient client;
        private readonly ILogger<PersistTrends> logger;

        public PersistTrends(DatabaseWriter.DatabaseWriterClient client, ILogger<PersistTrends> logger)
        {
            this.client=client;
            this.logger=logger;
        }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var trends = (TrendProviderReply)context.WorkflowInstance.Variables.Get(Nameservice.VariableNames.Trends)!;
            try
            {
                await client.StoreTrendsAsync(trends);
                logger.LogInformation($"Stored {trends.Trends.Count} Trends in Database!");
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Failed to Persist Trends with Error:\n{ex.Message}!");
                return Fault(ex);
            }
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
