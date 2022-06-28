using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{
    public class PersistSentiment : Activity
    {
        private readonly DatabaseWriter.DatabaseWriterClient client;
        private readonly ILogger<PersistSentiment> logger;

        public PersistSentiment(DatabaseWriter.DatabaseWriterClient client,ILogger<PersistSentiment> logger)
        {
            this.client=client;
            this.logger=logger;
        }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var gatheredTweets = context.WorkflowInstance.Variables.Get<Dictionary<string, List<Tweet>>>(Nameservice.VariableNames.CleanedTweets)!;
            var sentiment = context.WorkflowInstance.Variables.Get<Dictionary<long, float>>(Nameservice.VariableNames.Sentiments)!;

            List<SentimentPayload> sentimentPayloads = new ();
 
            try
            {
                foreach (var (topic, tweets) in gatheredTweets)
                {
                    foreach(var tweet in tweets)
                    {
                        if (sentiment.ContainsKey(tweet.ID))
                        {
                            var payload = new SentimentPayload();
                            payload.Topic = topic;
                            payload.Sentiment = sentiment[tweet.ID];
                            payload.Tweet = tweet;
                            sentimentPayloads.Add(payload);
                        }
                    }
                }

                if (sentimentPayloads.Count>0)
                {
                    var request = new StoreSentimentRequest();
                    request.Sentiments.Add(sentimentPayloads);
                    await client.StoreSentimentAsync(request);
                }
                else
                {
                    logger.Log(LogLevel.Warning, "Found no Sentiments to Store!");
                }

            }catch(Exception ex)
            {
                logger.Log(LogLevel.Error, $"Failed to Store Sentimenst with Error:\n{ex.Message}");
            }


            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
