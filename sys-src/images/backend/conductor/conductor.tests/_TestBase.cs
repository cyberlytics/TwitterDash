using conductor.background_services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace conductor.tests
{

    internal class MockingAsyncStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly IEnumerator<T> enumerator;

        public MockingAsyncStreamReader(IEnumerable<T> results)
        {
            enumerator = results.GetEnumerator();
        }

        public T Current => enumerator.Current;

        public Task<bool> MoveNext(CancellationToken cancellationToken) =>
            Task.Run(() => enumerator.MoveNext());
    }

    public class LogEnrty
    {
        public LogEnrty(LogLevel level, EventId id, string message, Exception ex, object func)
        {
            Level=level;
            Id=id;
            Message=message;
            Ex=ex;
            Func=func;
        }

        public LogLevel Level { get; }
        public EventId Id { get; }
        public string Message { get; }
        public Exception Ex { get; }
        public object Func { get; }
    }

    [TestFixture]
    public abstract class TestBase
    {
        protected List<LogEnrty> LogMessages = new();
        protected string TestDataDirectory => Path.Combine(TestContext.CurrentContext.TestDirectory, "test-data");

        [SetUp]
        public virtual void Setup()
        {
            LogMessages = new();
        }

        [TearDown]
        public virtual void Teardown()
        {

        }

        protected ILogger<TType> BuildLogger<TType>(){
            var loggerMock = new Mock<ILogger<TType>>();
            loggerMock.Setup(x => x.Log<It.IsAnyType>(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()))
                .Callback<LogLevel, EventId, object, Exception, object>(
                (level, id, message, ex, func) =>
                {
                    LogMessages.Add(new(level, id, message?.ToString(), ex, func));
                });
            return loggerMock.Object;
        }
    }
}