using MongoDB.Driver;
using MongoDB.Bson;

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

    public async Task<TwitterTrends> GetCurrentTrends(int woeid)
    {
        var sort = Builders<TwitterTrends>.Sort.Descending("DateTime");
        return await collection.Find(t => t.Country == woeid).Sort(sort).FirstAsync();
    }

    public async Task<List<TwitterTrends>> GetRecentTrends(DateTime? startDate, DateTime? endDate, string hashtag)
    {
        if (startDate != null && endDate != null)
        {
            return await collection.Find(x =>
            x.DateTime.Date < endDate
            && x.DateTime.Date > startDate
            && x.Trends.Any(y => y.name == hashtag))
            .ToListAsync();
        }
        else
        {
            return await collection.Find(x =>
             x.Trends.Any(y => y.name == hashtag))
            .ToListAsync();
        }
    }

    public async Task StoreTrends(TwitterTrends trends)
    {
        await collection.InsertOneAsync(trends);
    }
}