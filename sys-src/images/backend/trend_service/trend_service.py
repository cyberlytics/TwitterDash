from stub.TrendService_pb2_grpc import TrendProviderServicer
from stub.TrendService_pb2 import TrendProviderReply 
import stub.TrendService_pb2_grpc
import grpc 
from concurrent import futures

class TrendService(TrendProviderServicer):
    
    def OnNewTrends(self, request:TrendProviderReply, context):
        pass

   
   
if __name__ == '__main__':
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=1))
    stub.TrendService_pb2_grpc.add_RouteGuideServicer_to_server(TrendService(), server)
    server.add_insecure_port('[::]:50051')
    server.start()
    server.wait_for_termination()