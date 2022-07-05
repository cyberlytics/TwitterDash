using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net;
using DatabaseService.Controller;
using DatabaseService.Repositories;
using DatabaseService.Models;

var mongodb_port = Environment.GetEnvironmentVariable("MONGODB_PORT") ?? "27017";
var mongodb_ip = Environment.GetEnvironmentVariable("MONGODB_IP") ?? "localhost";
var mongodb_user = Environment.GetEnvironmentVariable("MONGODB_USER") ?? "root";
var mongodb_password = Environment.GetEnvironmentVariable("MONGODB_PASSWORD") ?? "example";
var grpc_port = int.Parse(Environment.GetEnvironmentVariable("GRPC_PORT") ?? "50051");

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
    });
    options.Listen(IPAddress.Any, grpc_port, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpc();

var client = new MongoClient($@"mongodb://{mongodb_user}:{mongodb_password}@{mongodb_ip}:{mongodb_port}/");
builder.Services.AddTransient<ITwitterTrendsRepository>((_) =>
    new TwitterTrendsRepository(
        client
        .GetDatabase("TwitterDash")
        .GetCollection<TwitterTrends>("Trends"), new woeid()));

builder.Services.AddTransient<ISentimentRepository>((_) =>
    new SentimentRepository(
        client
        .GetDatabase("TwitterDash")
        .GetCollection<Sentiment>("Sentiment")));


builder.Services.AddSingleton<woeid>((_) => new woeid());

var app = builder.Build();

app.UseRouting();
//app.UseHttpsRedirection();
//app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    app.MapGrpcService<DatabaseWriterController>();
    app.MapGrpcService<DatabaseReaderController>();
});

app.Run();