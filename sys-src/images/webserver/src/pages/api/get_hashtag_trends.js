// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
import protoLoader from "@grpc/proto-loader";
import grpc from "@grpc/grpc-js";

const USE_DUMMY_DATA = true

if (USE_DUMMY_DATA) {
    const fs = require('fs');
}
else {
    // within docker
    // const DB_SERVICE_HOSTNAME = process.env.DB_SERVICE_HOSTNAME;
    // const DB_SERVICE_PORT = process.env.DB_SERVICE_PORT;

    //outside docker
    const DB_SERVICE_HOSTNAME = "localhost";
    const DB_SERVICE_PORT = 50051;

    //const PROTO_PATH = './../../../protos/DatabaseService.proto';
    const PROTO_PATH = 'C:\\Users\\martin\\Documents\\GitHub\\bdcc-team-white\\sys-src\\images\\webserver\\protos\\DatabaseService.proto';
    const PROTO_DIR = 'C:\\Users\\martin\\Documents\\GitHub\\bdcc-team-white\\sys-src\\images\\webserver\\protos'
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

    let RecentTrendsRequest = {
        hashtag: "TonyAwards2022"
    };

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

    let client = new twitterdash.DatabaseReader(`${DB_SERVICE_HOSTNAME}:${DB_SERVICE_PORT}`, grpc.credentials.createInsecure());
}


export default function handler(req, res) {
    let num_results = req.query.num_results;
    console.log("api: num_results: " + num_results);
    if (USE_DUMMY_DATA) {
        let hashtag_trends = require('../../dummy_data/improved_hashtags.json');
        let dataSlice = hashtag_trends.slice(0, num_results)
        res.status(200).json(dataSlice);
    }
    else {
        client.GetCurrentTrends({limit: num_results}, dataCallback);
        //client.GetRecentTrends(RecentTrendsRequest, dataCallback);
        //client.GetAvailableCountries(null, dataCallback);
        //TODO: return data
        res.status(200).json({});
    }
}
