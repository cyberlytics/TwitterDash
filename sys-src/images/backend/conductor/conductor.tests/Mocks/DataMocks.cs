using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Twitterdash;

namespace conductor.tests.Mocks
{
    public static class DataMocks
    {
        public static TrendProviderReply LoadTrends(string directroy)
        {
            var trendsjson = Path.Combine(directroy, "trends-example.json");
            var jsonDoc = JsonDocument.Parse(File.ReadAllText(trendsjson));
            List<Twitterdash.Trend> trends = new();

            foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
            {
                trends.Add(new Twitterdash.Trend()
                {
                    Country = jsonElement.GetProperty("country").GetInt32(),
                    Name = jsonElement.GetProperty("hashtag").GetString(),
                    Placement =  jsonElement.GetProperty("top").GetInt32(),
                    TrendType = Twitterdash.TrendType.Hashtag,
                    TweetVolume24 = jsonElement.GetProperty("tweet_volume").GetInt32()
                });
            }

            var trendReply = new TrendProviderReply();
            trendReply.Trends.AddRange(trends);
            trendReply.Timestamp = new(DateTime.Now.ToUniversalTime().ToTimestamp());
            return trendReply;
        }

        public static List<Tweet> LoadTweets(string directroy)
        {
            var trendsjson = Path.Combine(directroy, "tweet_example.json");
            var jsonDoc = JsonDocument.Parse(File.ReadAllText(trendsjson));
            List<Tweet> tweets = new();

            foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
            {
                var tweet = new Tweet();
                tweet.Language = "de";
                tweet.ID = long.Parse(jsonElement.GetProperty("ID").GetString());
                tweet.ConversationID = long.Parse(jsonElement.GetProperty("Conversation_ID").GetString());
                tweet.Text = jsonElement.GetProperty("Text").GetString();
                tweet.Likes = jsonElement.GetProperty("likes").GetInt32();
                tweet.Replies = jsonElement.GetProperty("replies").GetInt32();
                tweet.Retweets = jsonElement.GetProperty("retweets").GetInt32();
                tweet.UserID = 0;
                tweet.Timestamp = DateTime.Parse(jsonElement.GetProperty("timestamp").ToString()).ToUniversalTime().ToTimestamp();
                tweet.Hashtags.AddRange(jsonElement.GetProperty("Hashtags").EnumerateArray().Select(x=>x.GetString()));
                tweets.Add(tweet);
            }
            return tweets;
        }

        public static Dictionary<string, List<Tweet>> BuildTrendTweetMap(string directory)
        {
            Dictionary<string, List<Tweet>> map = new();
            var trends = LoadTrends(directory);
            var tweets = LoadTweets(directory);
            var test = new HashSet<long>(tweets.Select(t => t.ID));
            var slice_size = tweets.Count/trends.Trends.Count;
            int i = 0;
            foreach (var trend in trends.Trends)
            {
                if (map.ContainsKey(trend.Name))
                    continue;

                map.Add(trend.Name, tweets.Skip(i*slice_size).Take(slice_size).ToList());
                i++;
            }
            return map;
        }

        public static Dictionary<long, float> BuildSentiment(Dictionary<string, List<Tweet>> tweetmap)
        {
            Random random = new Random(42);

            var allTweets = new List<Tweet>();
            foreach (var sublist in tweetmap.Values.ToList())
                allTweets.AddRange(sublist);

            var IDs = allTweets.Select(t => t.ID);

            var sentiments = new Dictionary<long, float>();

            foreach(var id in IDs)
            {
                if (sentiments.ContainsKey(id))
                    continue;

                sentiments.Add(id, ((float)random.NextDouble()));
            }
            return sentiments;

        }
    }
}
