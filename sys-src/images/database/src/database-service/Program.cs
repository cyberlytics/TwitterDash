using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Net;
using places;

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

builder.Services.AddTransient<MongoClient>((_) => new MongoClient($@"mongodb://{mongodb_user}:{mongodb_password}@{mongodb_ip}:{mongodb_port}/"));
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

// var client = new MongoClient($@"mongodb://{mongodb_user}:{mongodb_password}@{mongodb_ip}:{mongodb_port}/");
// var database = client.GetDatabase("TwitterDash");
// var collection = database.GetCollection<TwitterTrends>("Trends");

// // if (await collection.CountDocumentsAsync(new BsonDocument()) == 0)
// // {
// Console.WriteLine("Creating Entry");
// await collection.InsertOneAsync(new()
// {
//     DateTime = DateTime.Now,
//     Trends = new()
//        {
//            new TwitterTrend
//            {
//                trendType = TrendType.Hashtag,
//                name = "#GRPC",
//                placement = 1,
//            },
//            new TwitterTrend
//            {
//                trendType = TrendType.Hashtag,
//                name = "#Donald Trump",
//                placement = 2,
//            },
//            new TwitterTrend
//            {
//                trendType = TrendType.Hashtag,
//                name = DateTime.Now.ToString(),
//                placement = 3,
//            },
//        }
// });
// // }

// var filter = Builders<TwitterTrends>.Filter.Gte<DateTime>("DateTime", DateTime.Now.AddMinutes(-15));
// var sort = Builders<TwitterTrends>.Sort.Descending("DateTime");
// var results = await collection.Find<TwitterTrends>(filter).Sort(sort).FirstAsync();
// foreach (var trend in results.Trends.ToList())
// {
//     Console.WriteLine(trend.name);
// }