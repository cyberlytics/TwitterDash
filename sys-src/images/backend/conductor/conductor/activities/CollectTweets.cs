using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Google.Protobuf.WellKnownTypes;
using Twitterdash;

namespace conductor.activities
{
    public class CollectTweets : Activity
    {
        private readonly TweetProvider.TweetProviderClient client;
        private readonly ILogger<CollectTweets> logger;
        const int limit = 300;
        public CollectTweets(Twitterdash.TweetProvider.TweetProviderClient client, ILogger<CollectTweets> logger)
        {
            this.client=client;
            this.logger=logger;
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var trends = (TrendProviderReply)context.WorkflowInstance.Variables.Get(Nameservice.VariableNames.Trends)!;
            var tweets = new Dictionary<string, List<Tweet>>();
            foreach (var trend in trends.Trends)
            {
                try
                {
                    var time = DateTime.Now.ToUniversalTime();
                    var request = new GetTweetsRequest();
                    request.Since = time.AddDays(-1).ToTimestamp();
                    request.Until = time.ToTimestamp();
                    request.Limit = limit;
                    request.Trend = trend.Name;
                    request.Languages.Add("de");
                    request.Languages.Add("en");
                    var reply = await client.GetTweetsAsync(request);
                    if (tweets.ContainsKey(trend.Name))
                    {
                        tweets[trend.Name].AddRange(reply.Tweets);
                    }
                    else
                    {
                        tweets.Add(trend.Name, new(reply.Tweets.ToList()));
                    }
                    logger.LogInformation($"Got {reply.Tweets.Count} Tweets for Trend '{trend.Name}'!");
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Failed to get Tweets for Trend '{trend.Name}'! Error:\n{ex.Message}");
                    await Task.Delay(100);
                }
            }
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.GatheredTweets,tweets);
            return Outcome(Nameservice.Outcomes.Done);
        }    
    }
}

