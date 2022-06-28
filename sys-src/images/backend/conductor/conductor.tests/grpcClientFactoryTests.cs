using conductor.factories;
using Grpc.Net.Client;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitterdash;

namespace conductor.tests
{
    public class grpcClientFactoryTests : TestBase
    {
        [Test]
        public void grpcClientFactory_BuildClient_Builds_Client()
        {
            Assert.Multiple(() =>
            {
                string ip = "http://localhost:4242";
                TrendProvider.TrendProviderClient client = null;
                Assert.DoesNotThrow(() => client = grpcClientFactory.BuildClient<TrendProvider.TrendProviderClient>(ip));

                Assert.IsNotNull(client);
            });
        }

        [Test]
        public void grpcClientFactory_BuildChannel_Builds_Channel()
        {
            Assert.Multiple(() =>
            {
                string ip = "http://localhost:4242";
                GrpcChannel channel = null;
                Assert.DoesNotThrow(() => channel = grpcClientFactory.BuildChannel(ip));

                Assert.IsNotNull(channel);
                Assert.AreEqual("localhost:4242", channel.Target);
            });
        }
    }
}
