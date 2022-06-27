const LOCAL_DEVELOPMENT = false;
const DB_SERVICE_HOSTNAME = LOCAL_DEVELOPMENT ? "localhost" : process.env.DB_SERVICE_HOSTNAME;
const DB_SERVICE_PORT = LOCAL_DEVELOPMENT ? 50051 : process.env.DB_SERVICE_PORT;

const PROTO_PATH = __dirname + "/../../../../protos/DatabaseService.proto";
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

export const DATABASE_READER_CLIENT = new twitterdash.DatabaseReader(`${DB_SERVICE_HOSTNAME}:${DB_SERVICE_PORT}`, grpc.credentials.createInsecure());