using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Twitterdash;
using MongoDB.Driver;

internal class DatabaseReaderController : Twitterdash.DatabaseReader.DatabaseReaderBase
{
    
    private MongoClient client;

    public DatabaseReaderController(MongoClient client)
    {
        this.client = client;
    }

    public override Task<GetAvailableCountriesReply> GetAvailableCountries(Empty request, ServerCallContext context)
    {
        return base.GetAvailableCountries(request, context);
    }

    public override Task<TrendProviderReply> GetCurrentTrends(GetCurrentTrendsRequest request, ServerCallContext context)
    {
        return base.GetCurrentTrends(request, context);
    }

    public override Task<GetRecentTrendsReply> GetRecentTrends(GetRecentTrendsRequest request, ServerCallContext context)
    {
        return base.GetRecentTrends(request, context);
    }
}