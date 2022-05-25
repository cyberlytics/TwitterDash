from stub.TrendService_pb2_grpc import TrendProviderServicer
from stub.objects_pb2 import  TrendProviderReply,Trend,TrendType
import stub.objects_pb2
import stub.TrendService_pb2_grpc
import grpc 
from concurrent import futures
import time

class TrendService(TrendProviderServicer):
    
    def __init__(self) -> None:
        super().__init__()
        self.isActive = True
        
    #see https://github.com/melledijkstra/python-grpc-chat for "Event-System"
    def OnNewTrends(self, request:stub.TrendService_pb2_grpc.google_dot_protobuf_dot_empty__pb2.Empty, context)->TrendProviderReply:
        print(f"Connection from {context.peer()}")
        while self.isActive:
            reply = TrendProviderReply()
            trends = []
            for i in range(3):
                trends.append(Trend(name="test",country=i,placement = i, tweetVolume24 = i,trendType = TrendType.Hashtag ))
             
            reply.timestamp.GetCurrentTime()
            reply.trends.extend(trends)
            yield reply
            time.sleep(3)

   
   
if __name__ == '__main__':
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=2))
    stub.TrendService_pb2_grpc.add_TrendProviderServicer_to_server(TrendService(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()