from stub.TrendService_pb2_grpc import TweetCountsProviderServicer
from stub.TrendService_pb2 import (
    GetRecentTweetCountsReply,
    GetRecentTweetCountsRequest,
    TweetCount,
    Granularity,
)
import stub.objects_pb2
import stub.TrendService_pb2_grpc
import grpc
from concurrent import futures
from twitter_api_caller import Twitter_API_Caller
import json
from google.protobuf.timestamp_pb2 import Timestamp
import datetime


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

        if request.granularity == Granularity.hour:
            granularity = "hour"
        elif request.granularity == Granularity.day:
            granularity = "day"
        elif request.granularity == Granularity.minute:
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
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=2))
    stub.TrendService_pb2_grpc.add_TweetCountsProviderServicer_to_server(
        TweetCountsService(), server
    )
    server.add_insecure_port("0.0.0.0:50013")
    server.start()
    print("Started Tweet Counts Service")
    server.wait_for_termination()
