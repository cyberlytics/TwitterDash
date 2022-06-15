using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{
    public class PerformSentimentAnalysis : Activity
    {
        private readonly SentimentProvider.SentimentProviderClient client;
        private readonly ILogger<PerformSentimentAnalysis> logger;

        public PerformSentimentAnalysis(SentimentProvider.SentimentProviderClient client, ILogger<PerformSentimentAnalysis> logger)
        {
            this.client=client;
            this.logger=logger;
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var filteredTweets = context.WorkflowInstance.Variables.Get<Dictionary<string, List<Tweet>>>(Nameservice.VariableNames.CleanedTweets);
            var sentiments = new Dictionary<long, float>();
            bool server_is_online = false;

            while (!server_is_online)
            {
                try
                {
                    var request = new GetSentimentRequest();
                    request.Text = "a test request";
                    request.Language = "de";
                    var reply = await client.GetSentimentAsync(request);
                    server_is_online = true;
                }
                catch
                {
                    logger?.LogInformation($"Sentiment Server is still Starting");
                    await Task.Delay(500);
                }
            }
            foreach (var (topic, tweets) in filteredTweets)
            {

                logger?.LogInformation($"Starting Sentiment Analysis for Trend '{topic}'");
                foreach (var tweet in tweets)
                {
                    try
                    {
                        if (tweet.Language != "de")
                            continue;

                        if (sentiments.ContainsKey(tweet.ID))
                            continue;

                        var request = new GetSentimentRequest();
                        request.Text = tweet.Text;
                        request.Language = tweet.Language;
                        var reply = await client.GetSentimentAsync(request);
                        
                        sentiments.Add(tweet.ID, reply.Sentiment);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError($"Failed to get Sentiment for Tweet with ID '{tweet.ID}'!\n Error: {ex.Message}");
                    }
                }
                logger?.LogInformation($"Finished Sentiment Analysis for Trend '{topic}'");
            }
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.Sentiments, sentiments);
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
