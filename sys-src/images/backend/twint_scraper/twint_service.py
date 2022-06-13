from stub.TweetService_pb2_grpc import TweetProviderServicer
from stub.objects_pb2 import TweetProviderReply, Tweet
import stub.objects_pb2
import stub.TweetService_pb2_grpc
import grpc
from concurrent import futures
import time
import json
from twint_scraper import Twint_Scraper

class TweetService(TweetProviderServicer):
    def __init__(self) -> None:
        super().__init__()
        self.isActive = True
        
        self.caller = Twint_Scraper()

    def GetTweets(
        self,
        request: stub.TweetService_pb2_grpc.google_dot_protobuf_dot_empty__pb2.Empty,
        context,
    ) -> TweetProviderReply:
        print(f"Connection from {context.peer()}")
        while self.isActive:
            reply = TweetProviderReply()
            tweets = []


            for tweet in self.caller.searchTwint("corona", since="2022-06-10", until="2022-06-12", language="de", limit=100):
                tweets.append(
                    Tweet(
                        ID =tweet["id"],
                        Conversation_ID = tweet["conversation_id"],
                        timestamp = tweet["date"],
                        Text = tweet["tweet"],
                        UserID = tweet["user_id"],
                        replies = tweet["nreplies"],
                        retweets = tweet["nretweets"],
                        Hashtags = tweet["hashtags"],
                    )
                )

            reply.timestamp.GetCurrentTime()
            reply.tweets.extend(tweets)
            yield reply


if __name__ == "__main__":
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=2))
    stub.TweetService_pb2_grpc.add_TweetProviderServicer_to_server(
        TweetService(), server
    )
    server.add_insecure_port("0.0.0.0:50013")
    server.start()
    print("Started Tweet-Scraper Service")
    server.wait_for_termination()
