using Grpc.Core;
using Grpc.Net.Client;

namespace conductor.factories
{
    public static class grpcClientFactory
    {
        public static GrpcChannel BuildChannel(string ip) {
            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            return GrpcChannel.ForAddress(ip, new GrpcChannelOptions
            {
                HttpHandler = httpHandler
            });
        }
       
        public static TClient BuildClient<TClient>(string ip) where TClient: ClientBase<TClient>
        {
            var channel = grpcClientFactory.BuildChannel(ip);
            return (TClient)Activator.CreateInstance(typeof(TClient), channel);
        }
    }
}
