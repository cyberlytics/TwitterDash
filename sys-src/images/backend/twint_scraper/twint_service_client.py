from stub.TweetService_pb2_grpc import TweetProviderStub
import stub.objects_pb2
import stub.TweetService_pb2_grpc
import grpc


if __name__ == "__main__":
    channel = grpc.insecure_channel('127.0.0.1:50013')
    client = TweetProviderStub(channel)

    client.GetTweets(stub.TweetService_pb2_grpc.TweetRequest())

    for response in client.OnNewTrends(client.GetTweets(stub.TweetService_pb2_grpc.TweetRequest())):
        print(response)