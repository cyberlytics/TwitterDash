from stub.TweetService_pb2_grpc import TweetProviderServicer
from stub.TweetService_pb2 import GetTweetsRequest, GetTweetsReply
from google.protobuf.timestamp_pb2 import Timestamp
import stub.objects_pb2
import stub.TweetService_pb2_grpc
import grpc
from concurrent import futures
import time
import json
from twint_scraper import Twint_Scraper
import datetime

class TweetService(TweetProviderServicer):
    def __init__(self) -> None:
        super().__init__()
        
        self.caller = Twint_Scraper()

    def GetTweets(
        self,
        request: GetTweetsRequest,
        context,
    ) -> GetTweetsReply:
        print(f"Connection from {context.peer()}")
        reply = GetTweetsReply()
        tweets = []

        

        for tweet in self.caller.searchTwint(request.trend, since=request.since.ToDatetime().strftime("%Y-%m-%d"), until=request.until.ToDatetime().strftime("%Y-%m-%d"), language=request.languages[0], limit=request.limit):
            
            tmpTimestamp = Timestamp()
            tmpTimestamp.FromDatetime(datetime.datetime.strptime(tweet["date"], "%Y-%m-%d %H:%M:%S"))

            tmpvar = stub.objects_pb2.Tweet(
                    ID =tweet["id"],
                    Conversation_ID = tweet["conversation_id"],
                    timestamp = tmpTimestamp,
                    Text = tweet["tweet"],
                    UserID = tweet["user_id"],
                    likes = tweet["nlikes"],
                    replies = tweet["nreplies"],
                    retweets = tweet["nretweets"],
                    Hashtags = tweet["hashtags"],
                    language = tweet["language"]
                )
            
            tweets.append(tmpvar)

        #reply.timestamp.GetCurrentTime()
        reply.tweets.extend(tweets)
        return reply


if __name__ == "__main__":
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=2))
    stub.TweetService_pb2_grpc.add_TweetProviderServicer_to_server(
        TweetService(), server
    )
    server.add_insecure_port("0.0.0.0:50011")
    server.start()
    print("Started Tweet-Scraper Service")
    server.wait_for_termination()
