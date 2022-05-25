from stub.TrendService_pb2_grpc import TrendProviderServicer
from stub.objects_pb2 import TrendProviderReply, Trend, TrendType
import stub.objects_pb2
import stub.TrendService_pb2_grpc
import grpc
from concurrent import futures
import time
from twitter_api_caller import Twitter_API_Caller
import json


class TrendService(TrendProviderServicer):
    def __init__(self) -> None:
        super().__init__()
        self.isActive = True

        with open(
            "./sys-src/images/backend/trend_service/tokens.json", "r"
        ) as token_file:
            tokens = json.load(token_file)

        self.caller = Twitter_API_Caller(
            tokens["consumer_key_v1"],
            tokens["consumer_secret_v1"],
            tokens["access_token_v1"],
            tokens["access_token_secret_v1"],
            tokens["consumer_key_v2"],
            tokens["consumer_secret_v2"],
            tokens["access_token_v2"],
            tokens["access_token_secret_v2"],
            tokens["bearer_token_v2"],
        )

    # see https://github.com/melledijkstra/python-grpc-chat for "Event-System"
    def OnNewTrends(
        self,
        request: stub.TrendService_pb2_grpc.google_dot_protobuf_dot_empty__pb2.Empty,
        context,
    ) -> TrendProviderReply:
        print(f"Connection from {context.peer()}")
        while self.isActive:
            reply = TrendProviderReply()
            trends = []

            # Get the trending hashtags
            for trend in self.caller.getTrending():
                trends.append(
                    Trend(
                        name=trend["hashtag"],
                        country=trend["country"],
                        placement=trend["top"],
                        tweetVolume24=trend["tweet_volume"],
                        trendType=TrendType.Hashtag,
                    )
                )

            reply.timestamp.GetCurrentTime()
            reply.trends.extend(trends)
            yield reply
            time.sleep(60)


if __name__ == "__main__":
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=2))
    stub.TrendService_pb2_grpc.add_TrendProviderServicer_to_server(
        TrendService(), server
    )
    server.add_insecure_port("127.0.0.1:50051")
    server.start()
    server.wait_for_termination()
