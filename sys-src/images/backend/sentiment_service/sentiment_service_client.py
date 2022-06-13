from matplotlib.pyplot import text
from requests import request
from stub.SentimentService_pb2_grpc import SentimentProviderStub
from stub.SentimentService_pb2 import GetSentimentRequest, GetSentimentReply
from google.protobuf.timestamp_pb2 import Timestamp
import stub.objects_pb2
import stub.SentimentService_pb2_grpc
import grpc
import datetime


if __name__ == "__main__":
    channel = grpc.insecure_channel('127.0.0.1:50050')
    client = SentimentProviderStub(channel)
    
    #request = GetSentimentRequest(text="Ich liebe mein neues Auto.")
    request = GetSentimentRequest(text="Ich hasse mein neues Auto.")

    response = client.GetSentiment(request)

    print(response)
