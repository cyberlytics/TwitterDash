using conductor.activities;
using conductor.background_services;
using conductor.factories;
using conductor.workflows;
using Twitterdash;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var trendServiceIP = Environment.GetEnvironmentVariable("TRENDSERVICE_IP") ?? "localhost";
var trendServicePort = Environment.GetEnvironmentVariable("TRENDSERVICE_PORT") ?? "50051";

var dbServiceIP = Environment.GetEnvironmentVariable("DBSERVICE_IP") ?? "localhost";
var dbServicePort = Environment.GetEnvironmentVariable("DBSERVICE_PORT") ?? "50051";

var twintServiceIP = Environment.GetEnvironmentVariable("TWINTSERVICE_IP") ?? "localhost";
var twintServicePort = Environment.GetEnvironmentVariable("TWINTSERVICE_PORT") ?? "50150";

var sentimentServiceIP = Environment.GetEnvironmentVariable("SENTIMENTSERVICE_IP") ?? "localhost";
var sentimentServicePort = Environment.GetEnvironmentVariable("SENTIMENTSERVICE_PORT") ?? "50250";

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel().UseUrls("http://0.0.0.0:7777");

var elsaConfig = builder.Configuration.GetSection("Elsa");

builder.Services.AddElsa(elsa => elsa
                    .AddConsoleActivities()
                    .AddHttpActivities(elsaConfig.GetSection("Server").Bind)
                    .AddActivity<PersistTopicModels>()
                    .AddActivity<PersistSentiment>()
                    .AddActivity<PerformSentimentAnalysis>()
                    .AddActivity<PerformTopicModeling>()
                    .AddActivity<MakeTweetsUnique>()
                    .AddActivity<ValidateTrends>()
                    .AddActivity<PersistTrends>()
                    .AddActivity<CollectTweets>()
                    .AddWorkflow<TrendWorkflow>());

builder.Services.AddElsaApiEndpoints();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<TrendProvider.TrendProviderClient>((serviceProvider) =>
grpcClientFactory.BuildClient<TrendProvider.TrendProviderClient>($"http://{trendServiceIP}:{trendServicePort}"));

builder.Services.AddSingleton<DatabaseWriter.DatabaseWriterClient>((serviceProvider) =>
grpcClientFactory.BuildClient<DatabaseWriter.DatabaseWriterClient>($"http://{dbServiceIP}:{dbServicePort}"));

builder.Services.AddSingleton<SentimentProvider.SentimentProviderClient>((serviceProvider) =>
grpcClientFactory.BuildClient<SentimentProvider.SentimentProviderClient>($"http://{sentimentServiceIP}:{sentimentServicePort}"));

builder.Services.AddSingleton<TweetProvider.TweetProviderClient>((serviceProvider) =>
grpcClientFactory.BuildClient<TweetProvider.TweetProviderClient>($"http://{twintServiceIP}:{twintServicePort}"));

builder.Services.AddHostedService((sp) => sp.GetRequiredService<TrendProviderService>());
builder.Services.AddSingleton<TrendProviderService>();
builder.Services.AddSingleton<WorkflowController>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles() // For Dashboard.
    .UseHttpActivities()
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
        endpoints.MapControllers();

        // For Dashboard.
        endpoints.MapFallbackToPage("/_Host");
    });

app.Services.GetRequiredService<WorkflowController>();
app.Run();
