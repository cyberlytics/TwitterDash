using DatabaseService.Models;
using MongoDB.Driver;

namespace DatabaseService.Repositories
{
    public class SentimentRepository : ISentimentRepository
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

        public async Task<IEnumerable<Sentiment>> GetAvailableSentimentTrends(string query, int limit)
        {
            var dbQuery = collection.Find(x => x.Trend == query);
            if (limit > 0)
            {
                dbQuery = dbQuery.Limit(limit);
            }
            return await dbQuery.ToListAsync();
        }

        public Task<Sentiment> GetCurrentSentiment(string trendName)
        {
            throw new NotImplementedException();
        }

        public Task<Sentiment> GetRecentSentiment(string trendName, DateTime? start, DateTime? end)
        {
            throw new NotImplementedException();
        }

        public async Task StoreSentiment(IEnumerable<Sentiment> sentiments)
        {
            await collection.InsertManyAsync(sentiments);
        }
    }
}
