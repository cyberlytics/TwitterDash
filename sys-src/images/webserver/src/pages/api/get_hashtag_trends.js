// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
const DB_SERVICE_HOSTNAME = process.env.DB_SERVICE_HOSTNAME;
const DB_SERVICE_PORT = process.env.DB_SERVICE_PORT;
const fs = require('fs');
const PROTO_PATH = './../../../protos/DatabaseService.proto';
let grpc = require('@grpc/grpc-js');
let protoLoader = require('@grpc/proto-loader');
let packageDefinition = protoLoader.loadSync(
    PROTO_PATH,
    {keepCase: true,
      longs: String,
      enums: String,
      defaults: true,
      oneofs: true
    });

let protoDescriptor = grpc.loadPackageDefinition(packageDefinition);
// The protoDescriptor object has the full package hierarchy
let routeguide = protoDescriptor.routeguide;
let client = new routeguide.RouteGuide(`${DB_SERVICE_HOSTNAME}:${DB_SERVICE_PORT}`, grpc.credentials.createInsecure());
let defaultRequest = {
    "limit": 50
}

export default function handler(req, res) {
  //let hashtag_trends = require('../../dummy_data/improved_hashtags.json');
  let hashtag_trends = client.GetCurrentTrends(defaultRequest);
  res.status(200).json(hashtag_trends)
}
