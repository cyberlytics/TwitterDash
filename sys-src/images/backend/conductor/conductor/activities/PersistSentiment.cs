using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{
    public class PersistSentiment : Activity
    {
        private readonly DatabaseWriter.DatabaseWriterClient client;

        public PersistSentiment(DatabaseWriter.DatabaseWriterClient client)
        {
            this.client=client;
        }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
