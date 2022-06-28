from stub.TrendService_pb2_grpc import TweetCountsProviderStub
from stub.TrendService_pb2 import (
    GetRecentTweetCountsRequest,
    GetRecentTweetCountsReply,
    Granularity,
)

from google.protobuf.timestamp_pb2 import Timestamp
import stub.objects_pb2
import stub.TrendService_pb2_grpc
import grpc
import datetime


if __name__ == "__main__":
    channel = grpc.insecure_channel("127.0.0.1:50013")
    client = TweetCountsProviderStub(channel)

    timestamp_since = Timestamp()
    timestamp_since.FromDatetime(datetime.datetime.strptime("2022-06-11", "%Y-%m-%d"))

    timestamp_until = Timestamp()
    timestamp_until.FromDatetime(datetime.datetime.strptime("2022-06-12", "%Y-%m-%d"))

    request = GetRecentTweetCountsRequest(
        query="#corona",
        start_date=timestamp_since,
        end_date=timestamp_until,
        granularity=Granularity.hour,
    )

    response = client.GetRecentTweetCounts(request)

    for tc in response.tweetCounts:
        print(tc)
