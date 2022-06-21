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
        private readonly int max_retries;
        readonly HashSet<string> valid_languages = new() { "de","en","fr","nl" };
        public PerformSentimentAnalysis(SentimentProvider.SentimentProviderClient client, ILogger<PerformSentimentAnalysis> logger,int max_retries=10)
        {
            this.client=client;
            this.logger=logger;
            this.max_retries=max_retries;
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var filteredTweets = context.WorkflowInstance.Variables.Get<Dictionary<string, List<Tweet>>>(Nameservice.VariableNames.CleanedTweets)!;
            var sentiments = new Dictionary<long, float>();
            bool server_is_online = false;
            int retry_count = 0;
            while (!server_is_online && retry_count<max_retries)
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
                    logger?.LogInformation($"Sentiment Server is still Starting...");
                    retry_count++;
                    await Task.Delay(5000);
                }
            }
            foreach (var (topic, tweets) in filteredTweets)
            {

                logger?.LogInformation($"Starting Sentiment Analysis for Trend '{topic}'");
                foreach (var tweet in tweets)
                {
                    try
                    {
                        if (!valid_languages.Contains(tweet.Language))
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
