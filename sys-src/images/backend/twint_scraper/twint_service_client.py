from stub.TweetService_pb2_grpc import TweetProviderStub
from stub.TweetService_pb2 import GetTweetsRequest, GetTweetsReply

from google.protobuf.timestamp_pb2 import Timestamp
import stub.objects_pb2
import stub.TweetService_pb2_grpc
import grpc
import datetime


if __name__ == "__main__":
    channel = grpc.insecure_channel('127.0.0.1:50013')
    client = TweetProviderStub(channel)

    timestamp_since = Timestamp()
    timestamp_since.FromDatetime(datetime.datetime.strptime("2022-06-11", "%Y-%m-%d"))

    timestamp_until = Timestamp()
    timestamp_until.FromDatetime(datetime.datetime.strptime("2022-06-12", "%Y-%m-%d"))
    timestamp_since.ToDatetime().strftime("%Y-%m-%d")


    request = GetTweetsRequest(trend="corona", since=timestamp_since, until=timestamp_until, languages=["de"], limit=100)

    response = client.GetTweets(request)

    for tweet in response.tweets:
        print(tweet)
