from stub.TrendService_pb2_grpc import TrendProviderServicer
from stub.objects_pb2 import TrendProviderReply, Trend, TrendType
import stub.objects_pb2
import stub.TrendService_pb2_grpc
import grpc
from concurrent import futures
import time
from twitter_api_caller import Twitter_API_Caller
import json

from stub.TrendService_pb2_grpc import TweetCountsProviderServicer
from stub.TrendService_pb2 import (
    GetRecentTweetCountsReply,
    GetRecentTweetCountsRequest,
    TweetCount
)
import stub.objects_pb2
import stub.TrendService_pb2_grpc
from google.protobuf.timestamp_pb2 import Timestamp
import datetime


class TrendService(TrendProviderServicer):
    def __init__(self) -> None:
        super().__init__()
        self.isActive = True

        # with open(
        #     "./sys-src/images/backend/trend_service/tokens.json", "r"
        # ) as token_file:
        #     tokens = json.load(token_file)

        with open("./tokens.json", "r") as token_file:
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
            debug=False,
        )

    # see https://github.com/melledijkstra/python-grpc-chat for "Event-System"
    def OnNewTrends(
        self,
        request: stub.TrendService_pb2_grpc.google_dot_protobuf_dot_empty__pb2.Empty,
        context,
    ) -> TrendProviderReply:
        print(f"Connection from {context.peer()}")

        while self.isActive and context.is_active():
            reply = TrendProviderReply()
            trends = []

            lastCallTime = time.time()
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

            # sleep until last call is 15 mins ago and chech, if client is still connected
            remaining_time = int(60 * 15 - (time.time() - lastCallTime))

            for i in range(remaining_time):
                time.sleep(1)
                if context.is_active() == False:
                    print("Connection aborted", context.peer())
                    break

class TweetCountsService(TweetCountsProviderServicer):
    def __init__(self) -> None:
        super().__init__()

        with open("./tokens.json", "r") as token_file:
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
            debug=True,
        )

    def GetRecentTweetCounts(
        self,
        request: GetRecentTweetCountsRequest,
        context,
    ) -> GetRecentTweetCountsReply:
        print(f"Connection from {context.peer()}")

        reply = GetRecentTweetCountsReply()

        if request.granularity == stub.objects_pb2.Granularity.hour:
            granularity = "hour"
        elif request.granularity == stub.objects_pb2.Granularity.day:
            granularity = "day"
        elif request.granularity == stub.objects_pb2.Granularity.minute:
            granularity = "minute"

        for count in self.caller.getTweetCount(
            request.query,
            granularity,
            request.start_date.ToDatetime(),
            request.end_date.ToDatetime(),
        ):
            timestamp = Timestamp()
            timestamp.FromDatetime(
                datetime.datetime.fromisoformat(count["end"][:-1] + "+00:00")
            )
            reply.tweetCounts.append(
                TweetCount(
                    datetime=timestamp,
                    count=count["tweet_count"],
                )
            )

        return reply

if __name__ == "__main__":
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=4))
    stub.TrendService_pb2_grpc.add_TrendProviderServicer_to_server(
        TrendService(), server
    )
    stub.TrendService_pb2_grpc.add_TweetCountsProviderServicer_to_server(
        TweetCountsService(), server
    )
    server.add_insecure_port("0.0.0.0:50010")
    server.start()
    print("Started Trend Service")
    server.wait_for_termination()
