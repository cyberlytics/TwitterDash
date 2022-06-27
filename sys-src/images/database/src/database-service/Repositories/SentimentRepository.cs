using DatabaseService.Models;
using MongoDB.Driver;
using Twitterdash;

namespace DatabaseService.Repositories
{
    public partial class SentimentRepository : ISentimentRepository
    {
        private IMongoCollection<Sentiment> collection;

        public SentimentRepository(IMongoCollection<Sentiment> collection)
        {
            this.collection = collection;
        }

        public async Task<IEnumerable<long>> FilterStoredIds(IEnumerable<long> tweetIDs)
        {
            HashSet<long> queryTweets = new(tweetIDs);
            var existingTweets = await this.collection.FindAsync(x => queryTweets.Contains(x.Tweet_ID));
            return tweetIDs.Except(existingTweets.ToList().Select(x => x.Tweet_ID));
            // return await collection.Find(x => !queryTweets.Contains(x.Tweet_ID)).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetTrendsWithAvailableSentiment(string query, int limit)
        {
            IQueryable<string>? dbquery = null;

            await Task.Run(() =>
            {
                dbquery = collection
               .AsQueryable()
               .Select(x => x.Trend)
               .Distinct()
               .Where(trend => trend.Contains(query));
            });

            if (dbquery == null)
                return new List<string>();

            if (limit > 0)
            {
                dbquery = dbquery.Take(limit);
            }
            return dbquery.ToList();
        }

        public async Task<float> GetCurrentSentiment(string trendName)
        {
            var last_tweet = await collection
                .Find(x => x.Trend == trendName)
                .SortByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();
            if (last_tweet == null)
                return 0.0f;
            var end_time = last_tweet.Timestamp;
            var start_time = end_time.AddHours(-1);

            var sentiments = await GetRecentSentiments(trendName, start_time, end_time,Granularity.Hour);
            if (sentiments.Count() == 0)
                return 0.0f;
            return sentiments.First().Mean;
        }

        public async Task<IEnumerable<SentimentBatch>> GetRecentSentiments(string trendName, DateTime? start, DateTime? end, Twitterdash.Granularity granularity)
        {
            FilterDefinition<Sentiment> filter;
            var nameFilter = Builders<Sentiment>.Filter.Eq(x => x.Trend, trendName);
            if (start is not null && end is not null)
            {
                var dateFilter = Builders<Sentiment>.Filter.And(
                    Builders<Sentiment>.Filter.Gte(x => x.Timestamp, start),
                    Builders<Sentiment>.Filter.Lte(x => x.Timestamp, end)
                );
                filter = Builders<Sentiment>.Filter.And(nameFilter, dateFilter);
            }
            else
            {
                filter = nameFilter;
            }

            var sentiments = await collection.Find(filter).SortBy(x => x.Timestamp).ToListAsync();
            if (sentiments.Count() == 0)
                return new List<SentimentBatch>();

            var start_timestamp = sentiments.First().Timestamp;
            var end_timestamp = sentiments.Last().Timestamp;

            if (start_timestamp == end_timestamp)
                return new List<SentimentBatch>() {
                    new(){ Time = start_timestamp,
                    Mean = sentiments.Select(x=>x.Value).Sum()/sentiments.Count()
                    }
                };


            DateTime AdvanceBatch(DateTime start)
            {
                if (granularity == Twitterdash.Granularity.Hour)
                    return start.AddHours(1);
                else if (granularity == Twitterdash.Granularity.Day)
                    return start.AddDays(1);
                else
                    throw new Exception("Fuck OFF");
            }

            var batched_sentiment = new List<SentimentBatch>();

            end_timestamp = AdvanceBatch(end_timestamp);
            while (start_timestamp < end_timestamp)
            {
                var next_time = AdvanceBatch(start_timestamp);
                var values = sentiments.Where(s => s.Timestamp>=start_timestamp && s.Timestamp<next_time).Select(s => s.Value);

                if (values.Count() > 0)
                {
                    batched_sentiment.Add(new()
                    {
                        Time = start_timestamp,
                        Mean = values.Sum()/values.Count(),
                    });
                }
                start_timestamp = next_time;
            }
            return batched_sentiment;
        }

        public async Task StoreSentiment(IEnumerable<Sentiment> sentiments)
        {
            await collection.InsertManyAsync(sentiments);
        }
    }
}
