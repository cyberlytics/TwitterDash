using conductor.background_services;
using conductor.tests.Mocks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Twitterdash;

namespace conductor.tests
{
    public class TrendProviderServiceTests : TestBase
    {
        
        ILogger<TrendProviderService> logger;
        public override void Setup()
        {
            base.Setup();
            logger = BuildLogger<TrendProviderService>();
        }

        public override void Teardown()
        {
            base.Teardown();
        }

        [Test]
        public void TrendProviderService_Should_Be_Constructable()
        {
            Assert.DoesNotThrow(() =>
            {
                new TrendProviderService(new Mock<TrendProvider.TrendProviderClient>().Object,logger);
            });
        }

        [Test]
        public async Task TrendProviderService_Should_Be_Startable()
        {
            
            var service = new TrendProviderService(new Mock<TrendProvider.TrendProviderClient>().Object, logger);
            await service.StartAsync(CancellationToken.None);
            await Task.Delay(100);
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(LogMessages);
                Assert.AreEqual("Listening on GRPC-TrendService Stream.", LogMessages[0].Message);
            });
        }

        [Test]
        public async Task TrendProviderService_Should_Be_Stoppable()
        {
            var service = new TrendProviderService(new Mock<TrendProvider.TrendProviderClient>().Object, logger);
            await service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);
            Assert.IsTrue(service.ExecuteTask.IsCompleted);
        }

        [Test]
        public async Task TrendProviderService_Should_Not_Stop_On_Exception()
        {
            var service = new TrendProviderService(new Mock<TrendProvider.TrendProviderClient>().Object, logger);
            await service.StartAsync(CancellationToken.None);
            await Task.Delay(500);
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(LogMessages);
                var errormessage = LogMessages[1];
                Assert.IsTrue(errormessage.Message.Contains("Encountered Error while Reading from GRPC-TrendService Stream!"));
                Assert.IsFalse(service.ExecuteTask.IsCompleted);
            });
        }

        [Test]
        public async Task TrendProviderService_Fires_Event_When_Trends_Are_Recieved()
        {
            var mocks = new GrpcClientMocks();
            var service = new TrendProviderService(mocks.MockTrendProviderClient(), logger);

            TrendsRecievedEventArgs eventArgs = null;
            service.OnTrendsRecieved += (_, args) =>
            {
                eventArgs = args;
            };
            await service.StartAsync(CancellationToken.None);
            await Task.Delay(500);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(eventArgs);
            });
        }
    }
}
