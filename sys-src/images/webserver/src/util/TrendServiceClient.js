const USE_MOCKUP_BACKEND = false;
const TREND_SERVICE_HOSTNAME = USE_MOCKUP_BACKEND ? "localhost" : process.env.TREND_SERVICE_HOSTNAME;
const TREND_SERVICE_PORT = USE_MOCKUP_BACKEND ? 50010 : process.env.TREND_SERVICE_PORT;

//relative path as seen during next.js execution
const PROTO_PATH = __dirname + "/../../../../protos/TrendService.proto";
const PROTO_DIR = __dirname + "/../../../../protos";

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

export const TREND_SERVICE_CLIENT = new twitterdash.TweetCountsProvider(`${TREND_SERVICE_HOSTNAME}:${TREND_SERVICE_PORT}`, grpc.credentials.createInsecure());
