using MongoDB.Driver;
using Moq;

[TestFixture]
public class TwitterTrendsRepositoryTests
{
    private Mock<IMongoCollection<TwitterTrends>> mockCollection;

    [SetUp]
    public void Setup()
    {
        mockCollection = new Mock<IMongoCollection<TwitterTrends>>();
    }

    [Test]
    public async Task StoreTrends()
    {
        var repo = new TwitterTrendsRepository(mockCollection.Object);
        var trends = new TwitterTrends
        {
            ID = "id1",
            DateTime = new DateTime(2022, 1, 1, 12, 0, 0),
            Country = 1,
            Trends = new List<TwitterTrend> {
                new TwitterTrend {
                    trendType = TrendType.Hashtag,
                    name = "#test1",
                    woeid = 1,
                    placement = 1,
                    tweetVolume24 = 10
                },
                new TwitterTrend {
                    trendType = TrendType.Hashtag,
                    name = "#test2",
                    woeid = 1,
                    placement = 2,
                    tweetVolume24 = 5
                }
            }
        };

        await repo.StoreTrends(trends);

        Assert.DoesNotThrow(() =>
            mockCollection.Verify(c => c.InsertOneAsync(trends, null, default))
        );
    }
}
