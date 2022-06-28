const DB_SERVICE_HOSTNAME = "localhost";
const DB_SERVICE_PORT = 50051;

const PROTO_PATH = __dirname + "/../../protos/DatabaseService.proto";
const PROTO_DIR = __dirname + "/../../protos";

let grpc = require('@grpc/grpc-js');
let protoLoader = require('@grpc/proto-loader');
let packageDefinition = protoLoader.loadSync(
  PROTO_PATH,
  {keepCase: true,
    longs: String,
    enums: String,
    defaults: true,
    oneofs: true,
    includeDirs: [PROTO_DIR]
  });

let protoDescriptor = grpc.loadPackageDefinition(packageDefinition);
let twitterdash = protoDescriptor.twitterdash;

let client = new twitterdash.DatabaseReader(`${DB_SERVICE_HOSTNAME}:${DB_SERVICE_PORT}`, grpc.credentials.createInsecure());

let dataCallback = (error, data) => {
  if (error) {
    console.log("Error occured!");
    console.log(error);
  }
  else {
    console.log("Success!");
    console.log(data);
  }
};

let GetRecentTrendsCallback = (error, data) => {
  if (error) {
    console.log("Error occured!");
    console.log(error);
  }
  else {
    console.log("Success!");
    for (const recentTrend of data.recentTrends) {
      console.log(recentTrend);
    }
  }
};

function main() {
  //client.GetAvailableCountries(null, dataCallback);
  //client.GetCurrentTrends({limit: 2, country:"Germany"}, dataCallback);
  // client.GetRecentTrends({hashtag: "#TEST123", country:"Germany"}, GetRecentTrendsCallback);
  // client.GetTrendsWithAvailableSentiment({query: "#BDCC", limit:10}, dataCallback)
  // client.GetCurrentSentiment({trendName: "#BDCC"}, dataCallback);
  client.GetRecentSentiments({trendName: "#BDCC", granularity: "hour"}, dataCallback);
}

if (require.main === module) {
  main();
}
