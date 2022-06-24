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

        public async Task<Sentiment> GetCurrentSentiment(string trendName)
        {
            return await collection
                .Find(x => x.Trend == trendName)
                .SortByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Sentiment>> GetRecentSentiment(string trendName, DateTime? start, DateTime? end)
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
            return await collection.Find(filter).SortByDescending(x => x.Timestamp).ToListAsync();
        }

        public async Task StoreSentiment(IEnumerable<Sentiment> sentiments)
        {
            await collection.InsertManyAsync(sentiments);
        }
    }
}
