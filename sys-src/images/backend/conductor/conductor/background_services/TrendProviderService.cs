using Twitterdash;

namespace conductor.background_services
{

    public class TrendsRecievedEventArgs : EventArgs
    {
        public TrendProviderReply Reply { get; private set; }

        public TrendsRecievedEventArgs(TrendProviderReply reply)
        {
            Reply = reply;
        }
    }

    public class TrendProviderService : BackgroundService
    {
        private readonly TrendProvider.TrendProviderClient client;
        private readonly ILogger<TrendProviderService> logger;
        public delegate void TrendsRecievedHandler(object sender, TrendsRecievedEventArgs e);
        public event TrendsRecievedHandler? OnTrendsRecieved;

        public TrendProviderService(TrendProvider.TrendProviderClient client, ILogger<TrendProviderService> logger)
        {
            this.client=client;
            this.logger=logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Log(LogLevel.Information,"Listening on GRPC-TrendService Stream.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var stream = this.client.OnNewTrends(new()).ResponseStream;
                    while(await stream.MoveNext(stoppingToken))
                    {
                        var trends = stream.Current;
                        OnTrendsRecieved?.Invoke(this, new(trends));
                    }
                }
                catch(Exception ex)
                {
                    logger.LogError($"Encountered Error while Reading from GRPC-TrendService Stream!\n{ex}");
                    await Task.Delay(2000);
                }
            }
        }
    }
}
