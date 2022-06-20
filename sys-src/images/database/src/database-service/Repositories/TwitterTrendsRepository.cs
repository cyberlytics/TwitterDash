using MongoDB.Driver;
using MongoDB.Bson;
using DatabaseService.Models;

namespace DatabaseService.Repositories
{
    public class TwitterTrendsRepository : ITwitterTrendsRepository
    {
        private IMongoCollection<TwitterTrends> collection;

        public TwitterTrendsRepository(IMongoCollection<TwitterTrends> collection)
        {
            this.collection = collection;
        }

        public async Task<List<int>> GetAvailableCountries()
        {
            var countries = await this.collection.DistinctAsync<int>("Country", new BsonDocument());
            return await countries.ToListAsync();
        }

        public async Task<TwitterTrends> GetCurrentTrends(int? woeid)
        {
            var sort = Builders<TwitterTrends>.Sort.Descending("DateTime");
            if (woeid == null)
            {
                return await collection.Find(Builders<TwitterTrends>.Filter.Empty).Sort(sort).FirstOrDefaultAsync();
            }
            else
            {
                return await collection.Find(t => t.Country == woeid).Sort(sort).FirstOrDefaultAsync();
            }
        }

        public async Task<List<TwitterTrends>> GetRecentTrends(DateTime? startDate, DateTime? endDate, string hashtag)
        {
            List<TwitterTrends> result = new List<TwitterTrends>();
            if (startDate != null && endDate != null && startDate < endDate)
            {
                result =  await collection.Find(x =>
                x.DateTime < endDate
                && x.DateTime > startDate
                && x.Trends.Any(y => y.name == hashtag))
                .ToListAsync();
            }
            else
            {
                result =  await collection.Find(x =>
                 x.Trends.Any(y => y.name == hashtag))
                .ToListAsync();
            }
            for (int i = 0; i < result.Count; i++)
            {
                result[i].Trends = result[i].Trends.Where(x => x.name == hashtag).ToList();
            }
            return result;
        }

        public async Task StoreTrends(TwitterTrends trends)
        {
            await collection.InsertOneAsync(trends);
        }
    }
}