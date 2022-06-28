using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Twitterdash;

namespace conductor.activities
{
    public class MakeTweetsUnique : Activity
    {
        private readonly DatabaseReader.DatabaseReaderClient client;
        private readonly ILogger<MakeTweetsUnique> logger;

        public MakeTweetsUnique(DatabaseReader.DatabaseReaderClient client,ILogger<MakeTweetsUnique> logger)
        {
            this.client=client;
            this.logger=logger;
        }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var gatheredTweets = context.WorkflowInstance.Variables.Get<Dictionary<string, List<Tweet>>>(Nameservice.VariableNames.GatheredTweets)!;
            var cleanedTweets = new Dictionary<string, List<Tweet>>();
            try
            {
                List<long> IDs = new();
                foreach(var (topic,tweets) in gatheredTweets)
                {
                    IDs.AddRange(tweets.Select(t=>t.ID));
                }

                var request = new GetUniqueTweetsPayload();
                request.TweetIds.Add(IDs);

                var response = await client.GetUniqueTweetsAsync(request);

                //Iterate the ids and only take tweets with matching IDs
                HashSet<long> uniqueIDs = new(response.TweetIds);
                foreach (var (topic, tweets) in gatheredTweets)
                {
                   var filteredTweets = new List<Tweet>();

                   foreach(var tweet in tweets)
                   {
                        if(uniqueIDs.Contains(tweet.ID))
                            filteredTweets.Add(tweet);
                   }

                    if (filteredTweets.Count > 0)
                        cleanedTweets.Add(topic, filteredTweets);
                }
                logger.Log(LogLevel.Information, $"Succesfully cleaned the gathered Tweets!");
            }
            catch(Exception ex)
            {
                logger.Log(LogLevel.Error, $"Error occured while filtering Tweets! Defaulting to gathered Tweets!\n{ex.Message}");
                cleanedTweets = gatheredTweets;
            }

            context.WorkflowInstance.Variables.Remove(Nameservice.VariableNames.GatheredTweets);
            context.WorkflowInstance.Variables.Set(Nameservice.VariableNames.CleanedTweets, cleanedTweets);
            return Outcome(Nameservice.Outcomes.Done);
        }
    }
}
