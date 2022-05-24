using MongoDB.Driver;

MongoClient client = new MongoClient($@"mongodb://root:example@mongo:27017/");

var database = client.GetDatabase("TwitterDash");
var collection = database.GetCollection<TwitterTrend>("Trends");

await collection.InsertOneAsync(new()
{
    DateTime = DateTime.Now,
    Trends = new()
    {
        { "#GRPC", 1 },
        { "#Donald Trump", 2 },
    }
});

var results = await collection.FindAsync(_ => true);
Console.WriteLine(results);