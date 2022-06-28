using DatabaseService.Models;

namespace DatabaseService.Repositories
{
    public interface ISentimentRepository
    {
        Task StoreSentiment(IEnumerable<Sentiment> sentiments);
        Task<IEnumerable<long>> FilterStoredIds(IEnumerable<long> tweetIDs);
        Task<IEnumerable<string>> GetTrendsWithAvailableSentiment(string query, int limit);
        Task<float> GetCurrentSentiment(string trendName);
        Task<IEnumerable<SentimentBatch>> GetRecentSentiments(string trendName, DateTime? start, DateTime? end, Twitterdash.Granularity granularity);
    }
}