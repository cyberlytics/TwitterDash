using Twitterdash;

public interface ITwitterTrendsRepository
{
    Task StoreTrends(TwitterTrends trends);
    Task<List<int>> GetAvailableCountries();
    Task<TwitterTrends> GetCurrentTrends(int woeid);
    Task<List<TwitterTrends>> GetRecentTrends(DateTime start, DateTime end, string hashtag);
}