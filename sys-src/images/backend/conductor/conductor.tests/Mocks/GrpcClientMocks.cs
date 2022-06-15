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
        public List<GetTweetsRequest> TweetProvider_GetTweetsAsync_Calls = new();
        public List<GetSentimentRequest> SentimentProvider_GetSentimentRequest_Calls = new();
        
        public SentimentProvider.SentimentProviderClient MockSentimentProviderClient(bool throws = false)
        {
            var SentimentProvider_mock = new Mock<SentimentProvider.SentimentProviderClient>();

            AsyncUnaryCall<GetSentimentReply> build_call(GetSentimentRequest request)
            {
                var reply = new GetSentimentReply();
                reply.Sentiment = 0.0F;
                return TestCalls.AsyncUnaryCall<GetSentimentReply>(
               Task.FromResult(reply),
               Task.FromResult(new Metadata()),
               () => Status.DefaultSuccess,
               () => new Metadata(),
               () => { });
            }

            if (throws)
            {
                SentimentProvider_mock.Setup(g => g.GetSentimentAsync(
                It.IsAny<GetSentimentRequest>(),
                null,
                null,
                CancellationToken.None))
                    .Throws(new Grpc.Core.RpcException(Status.DefaultCancelled));
            }
            else
            {
                SentimentProvider_mock.Setup(g => g.GetSentimentAsync(
                 It.IsAny<GetSentimentRequest>(),
                 null,
                 null,
                 CancellationToken.None))
                    .Returns<GetSentimentRequest, object, object, object>
                        ((GetSentimentRequest, Metadata, DateTime, CancellationToken) =>
                        build_call(GetSentimentRequest))
                    .Callback<GetSentimentRequest, object, object, object>
                        ((GetSentimentRequest, Metadata, DateTime, CancellationToken) =>
                        {
                            SentimentProvider_GetSentimentRequest_Calls.Add(GetSentimentRequest);
                        });
            }

            return SentimentProvider_mock.Object;
        }
        public TweetProvider.TweetProviderClient MockTweetProviderClient(string testDataDirectory,bool throws = false)
        {
            var TweetProvider_mock = new Mock<TweetProvider.TweetProviderClient>();

            var tweetTrendMap = DataMocks.BuildTrendTweetMap(testDataDirectory);

            AsyncUnaryCall<GetTweetsReply> build_call(GetTweetsRequest request)
            {
                var reply = new GetTweetsReply();
                reply.Tweets.AddRange(tweetTrendMap[request.Trend]);
                return TestCalls.AsyncUnaryCall<GetTweetsReply>(
               Task.FromResult(reply),
               Task.FromResult(new Metadata()),
               () => Status.DefaultSuccess,
               () => new Metadata(),
               () => { });
            }

            if (throws)
            {
                TweetProvider_mock.Setup(g => g.GetTweetsAsync(
                It.IsAny<GetTweetsRequest>(),
                null,
                null,
                CancellationToken.None))
                    .Throws(new Grpc.Core.RpcException(Status.DefaultCancelled));
            }
            else
            {
                TweetProvider_mock.Setup(g => g.GetTweetsAsync(
                 It.IsAny<GetTweetsRequest>(),
                 null,
                 null,
                 CancellationToken.None))
                    .Returns<GetTweetsRequest, object, object, object>
                        ((GetTweetsRequest, Metadata, DateTime, CancellationToken) => 
                        build_call(GetTweetsRequest))
                    .Callback<GetTweetsRequest, object, object, object>
                        ((GetTweetsRequest, Metadata, DateTime, CancellationToken) =>
                        {
                            TweetProvider_GetTweetsAsync_Calls.Add(GetTweetsRequest);
                        });
            }
            return TweetProvider_mock.Object;
        }

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
