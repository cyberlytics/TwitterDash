const TREND_SERVICE_HOSTNAME = "localhost";
const TREND_SERVICE_PORT = 50010;

const PROTO_PATH = __dirname + "/../../protos/TrendService.proto";
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
let client = new twitterdash.TweetCountsProvider(`${TREND_SERVICE_HOSTNAME}:${TREND_SERVICE_PORT}`, grpc.credentials.createInsecure());

const GRANULARITY = {
    minute: 0,
    hour: 1,
    day: 2
}

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


function main() {
  client.GetRecentTweetCounts({query: "#BDCC", granularity: "minute"}, dataCallback);
}

if (require.main === module) {
  main();
}
