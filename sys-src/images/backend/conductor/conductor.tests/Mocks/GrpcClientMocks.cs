using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Twitterdash;

namespace conductor.tests.Mocks
{
    public class GrpcClientMocks
    {
        public List<TrendProviderReply> DatabaseWriterClient_StoreTrendsAsync_Calls = new();

        public DatabaseWriter.DatabaseWriterClient MockDatabaseWriterClient(bool throws = false)
        {
            var DatabaseWriter_mock = new Mock<DatabaseWriter.DatabaseWriterClient>();

            var mocked_call = TestCalls.AsyncUnaryCall<Empty>(
               Task.FromResult(new Empty()),
               Task.FromResult(new Metadata()),
               () => Status.DefaultSuccess,
               () => new Metadata(),
               () => { });

            if (throws)
            {
                DatabaseWriter_mock.Setup(g => g.StoreTrendsAsync(
                It.IsAny<TrendProviderReply>(),
                null,
                null,
                CancellationToken.None))
                    .Throws(new Grpc.Core.RpcException(Status.DefaultCancelled));
            }
            else
            {
                DatabaseWriter_mock.Setup(g => g.StoreTrendsAsync(
                 It.IsAny<TrendProviderReply>(),
                 null,
                 null,
                 CancellationToken.None))
                    .Returns(mocked_call)
                    .Callback<TrendProviderReply, object, object, object>
                    ((TrendProviderReply, Metadata, DateTime, CancellationToken) =>
                    {
                        DatabaseWriterClient_StoreTrendsAsync_Calls.Add(TrendProviderReply);
                    });
            }
            return DatabaseWriter_mock.Object;
        }

        public TrendProvider.TrendProviderClient MockTrendProviderClient()
        {
            var grpc_mock = new Mock<TrendProvider.TrendProviderClient>();

            var trendReply = new TrendProviderReply();
            trendReply.Trends.Add(new Trend()
            {
                Country = 0,
                Name = "#UNITTESTING",
                Placement = 1,
                TweetVolume24 = int.MaxValue,
                TrendType = TrendType.Hashtag
            });

            var trendResponses = new List<TrendProviderReply>()
            {
                trendReply
            };
            var reader = new MockingAsyncStreamReader<TrendProviderReply>(trendResponses);
            var mocked_server_stream = TestCalls.AsyncServerStreamingCall(
                reader, // Pass the stream reader into the gRPC call
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
            grpc_mock.Setup(g => g.OnNewTrends(It.IsAny<Empty>(), null, null, CancellationToken.None)).Returns(mocked_server_stream);
            return grpc_mock.Object;
        }
    }
}
