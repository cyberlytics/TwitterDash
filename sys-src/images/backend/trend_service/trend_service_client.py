from stub.TrendService_pb2_grpc import TrendProviderStub
import stub.objects_pb2
import stub.TrendService_pb2_grpc
import grpc



if __name__ == "__main__":
    channel = grpc.insecure_channel('localhost:50051')
    client = TrendProviderStub(channel)
    for response in client.OnNewTrends(stub.TrendService_pb2_grpc.google_dot_protobuf_dot_empty__pb2.Empty()):
        print(response)
    