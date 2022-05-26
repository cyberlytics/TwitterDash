using MongoDB.Driver;

var mongodb_port = Environment.GetEnvironmentVariable("MONGODB_PORT") ?? "27017";
var mongodb_ip = Environment.GetEnvironmentVariable("MONGODB_IP") ?? "localhost";
var mongodb_user = Environment.GetEnvironmentVariable("MONGODB_USER") ?? "root";
var mongodb_password = Environment.GetEnvironmentVariable("MONGODB_PASSWORD") ?? "example";

MongoClient client = new MongoClient($@"mongodb://{mongodb_user}:{mongodb_password}@{mongodb_ip}:{mongodb_port}/");

var database = client.GetDatabase("TwitterDash");
var collection = database.GetCollection<TwitterTrend>("Trends");

while (true)
{
    Console.WriteLine("Creating Entry");
    await collection.InsertOneAsync(new()
    {
        DateTime = DateTime.Now,
        Trends = new()
        {
            { "#GRPC", 1 },
            { "#Donald Trump", 2 },
        }
    });
    Task.Delay(3000).Wait();
}

var results = await collection.FindAsync(_ => true);
Console.WriteLine(results);