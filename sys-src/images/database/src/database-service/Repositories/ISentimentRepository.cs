using DatabaseService.Models;

namespace DatabaseService.Repositories
{
    public interface ISentimentRepository
    {
        Task StoreSentiment(IEnumerable<Sentiment> sentiments);
        Task<IEnumerable<long>> FilterStoredIds(IEnumerable<long> tweetIDs);
        Task<IEnumerable<Sentiment>> GetAvailableSentimentTrends(string query, int limit);
        Task<Sentiment> GetCurrentSentiment(string trendName);
        Task<IEnumerable<Sentiment>> GetRecentSentiment(string trendName, DateTime? start, DateTime? end);
    }
}