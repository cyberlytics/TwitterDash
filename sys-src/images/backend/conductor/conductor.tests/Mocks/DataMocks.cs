using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
