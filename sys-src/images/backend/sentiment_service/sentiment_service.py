from stub.SentimentService_pb2_grpc import SentimentProviderServicer
from stub.SentimentService_pb2 import GetSentimentRequest, GetSentimentReply
from google.protobuf.timestamp_pb2 import Timestamp
import stub.objects_pb2
import stub.SentimentService_pb2_grpc
import grpc
from concurrent import futures
import time
import json
from sentiment import Sentiment_Service_Transformer, Sentiment_Service_Blob
import datetime


class SentimentService(SentimentProviderServicer):
    def __init__(self) -> None:
        super().__init__()

        # BERT-Modell
        self.caller = Sentiment_Service_Transformer()
        self.caller.init_model()
        self.caller.load_model()
        # # Blob Modell
        # self.caller = Sentiment_Service_Blob()

    def GetSentiment(self, request: GetSentimentRequest, context) -> GetSentimentReply:
        print(f"Connection from {context.peer()}")
        reply = GetSentimentReply()

        sentiment = self.caller.get_sentiment(request.text, request.language)

        reply.sentiment = sentiment
        return reply


if __name__ == "__main__":
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=2))
    stub.SentimentService_pb2_grpc.add_SentimentProviderServicer_to_server(
        SentimentService(), server
    )
    server.add_insecure_port("0.0.0.0:50012")
    server.start()
    print("Started Sentiment Service")
    server.wait_for_termination()
