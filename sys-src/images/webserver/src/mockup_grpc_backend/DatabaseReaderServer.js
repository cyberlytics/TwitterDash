const DB_SERVICE_HOSTNAME = "localhost";
const DB_SERVICE_PORT = 50051;

const PROTO_PATH = __dirname + "/../../protos/DatabaseService.proto";
const PROTO_DIR = __dirname + "/../../protos";

let grpc = require('@grpc/grpc-js');
let protoLoader = require('@grpc/proto-loader');
const gs = require("@google-cloud/scheduler");

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

const TREND_TYPE = {
  Topic: 0,
  Hashtag: 1
}

const WOEIDS = {
  Worldwide: 1,
  Switzerland: 23424957,
  Germany: 23424829,
  USA: 23424977,
  UK: 23424975,
  AUSTRIA: 23424750
}

let now_seconds = Math.floor(Date.now() / 1000)

function genRandomString(len) {
  let chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
  let res = "";
  for (let i = 0; i < len; ++i) {
    res += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return res;
}

function generateRandomTrend(placement, country, name=null) {
  let trendType = null;

  if (name == null) {
    trendType = Math.round(Math.random());
    let randomLength = Math.floor(Math.random() * 8) + 4;
    name = genRandomString(randomLength);
    if (trendType == TREND_TYPE.Hashtag) {
      name = "#" + name;
    }
  }
  else {
    trendType = name.startsWith("#") ? TREND_TYPE.Hashtag : TREND_TYPE.Topic;
  }

  let randomTweetVolume = Math.floor(Math.random() * 1000000);

  return {
    trendType: trendType,
    name: name,
    country: WOEIDS[country], // WOEID of the country
    placement: placement, // Place of the Tweet in the given Country
    tweetVolume24: randomTweetVolume // Number of Tweets in the given Country
  }
}

async function GetCurrentTrendsInternal(GetCurrentTrendsRequest) {
  let country = null;
  if (GetCurrentTrendsRequest.hasOwnProperty("country")) {
    country = GetCurrentTrendsRequest.country;
  }
  else {
    country = "Worldwide";
  }
  let limit = GetCurrentTrendsRequest.limit;

  let trends = [];
  for (let i = 1; i <= limit ; ++i) {
    trends.push(generateRandomTrend(i, country));
  }

  const current_timestamp = new gs.protos.google.protobuf.Timestamp.fromObject({seconds: Math.floor(Date.now() / 1000)});

  let TrendProviderReply = {
    timestamp: current_timestamp,
    trends: trends
  }
  return TrendProviderReply;
}

async function GetCurrentTrends(call, callback) {
  callback(null, await GetCurrentTrendsInternal(call.request));
}

async function GetAvailableCountriesInternal() {
  let GetAvailableCountriesReply = {
    countries: Object.keys(WOEIDS)
  };

  return GetAvailableCountriesReply;
}

async function GetAvailableCountries(call, callback) {
  callback(null, await GetAvailableCountriesInternal());
}

async function GetRecentTrendsInternal(GetRecentTrendsRequest) {
  let hashtag = GetRecentTrendsRequest.hashtag;
  let end_date = new gs.protos.google.protobuf.Timestamp.fromObject({seconds: Math.floor(Date.now() / 1000)});
  if (GetRecentTrendsRequest.hasOwnProperty("end_date")) {
    end_date = new gs.protos.google.protobuf.Timestamp.fromObject(GetRecentTrendsRequest.end_date);
  }

  let start_date = new gs.protos.google.protobuf.Timestamp.fromObject({seconds: end_date.seconds - 7 * 24 * 60 * 60});
  if (GetRecentTrendsRequest.hasOwnProperty("start_date")) {
    start_date = new gs.protos.google.protobuf.Timestamp.fromObject(GetRecentTrendsRequest.start_date);
  }

  let country = null;
  if (GetRecentTrendsRequest.hasOwnProperty("country")) {
    country = GetRecentTrendsRequest.country;
  }
  else {
    country = "Worldwide";
  }
  let recentTrends = [];
  let no_data_prob = 0.8;
  for (let i = start_date.seconds.low; i <= end_date.seconds.low ; i += 15 * 60) {
    if (i > now_seconds) {
      break;
    }
    if (Math.random() > no_data_prob) {
      let randomPlacement = Math.floor(Math.random() * 50);
      let trend = generateRandomTrend(randomPlacement, country, hashtag);
      let timestamp = new gs.protos.google.protobuf.Timestamp.fromObject({seconds: i});
      let recentTrend = {
        datetime: timestamp,
        trend: trend
      }
      recentTrends.push(recentTrend)
    }
  }

  let GetRecentTrendsReply = {
    recentTrends: recentTrends
  }

  return GetRecentTrendsReply;
}

async function GetRecentTrends(call, callback) {
  callback(null, await GetRecentTrendsInternal(call.request));
}

async function GetTrendsWithAvailableSentimentInternal(GetTrendsWithAvailableSentimentRequest) {
  let query = GetTrendsWithAvailableSentimentRequest.query;
  let limit = GetTrendsWithAvailableSentimentRequest.limit;
  let num_available = Math.floor(Math.random() * 100) + 1
  let availableTrendsWithSentiment = []
  for (let i = 0; i < num_available; ++i) {
    let randomString = genRandomString(Math.floor(Math.random() * 10) + 1)
    availableTrendsWithSentiment.push(query + randomString);
  }
  availableTrendsWithSentiment = availableTrendsWithSentiment.slice(0, limit);
  let GetTrendsWithAvailableSentimentReply = {
    availableTrendsWithSentiment
  }

  return GetTrendsWithAvailableSentimentReply
}

async function GetTrendsWithAvailableSentiment(call, callback) {
  callback(null, await GetTrendsWithAvailableSentimentInternal(call.request));
}

async function GetCurrentSentimentInternal(GetCurrentSentimentRequest) {
  let trendName = GetCurrentSentimentRequest.trendName;
  let GetCurrentSentimentReply = {
    sentiment: Math.random() * 2 - 1
  }
  return GetCurrentSentimentReply;
}

async function GetCurrentSentiment(call, callback) {
  callback(null, await GetCurrentSentimentInternal(call.request));
}

async function GetRecentSentimentsInternal(GetRecentSentimentsRequest) {
  let trendName = GetRecentSentimentsRequest.trendName;
  let granularity = GetRecentSentimentsRequest.granularity;
  let end_date = new gs.protos.google.protobuf.Timestamp.fromObject({seconds: Math.floor(Date.now() / 1000)});
  if (GetRecentSentimentsRequest.hasOwnProperty("end_date")) {
    end_date = new gs.protos.google.protobuf.Timestamp.fromObject(GetRecentSentimentsRequest.end_date);
  }

  let start_date = new gs.protos.google.protobuf.Timestamp.fromObject({seconds: end_date.seconds - 7 * 24 * 60 * 60});
  if (GetRecentSentimentsRequest.hasOwnProperty("start_date")) {
    start_date = new gs.protos.google.protobuf.Timestamp.fromObject(GetRecentSentimentsRequest.start_date);
  }

  let granularity_seconds = null;
  switch (granularity) {
    case "day":
      granularity_seconds = 60 * 60 * 24;
      break;
    case "hour":
      granularity_seconds = 60 * 60;
      break;
    case "minute":
      granularity_seconds = 60;
      break;
  }

  let recentSentiments = [];
  for (let i = start_date.seconds.low; i <= end_date.seconds.low ; i += granularity_seconds) {
    if (i < now_seconds) {
      let no_data_prob = 0.2;
      if (Math.random() > no_data_prob) {
        let sentiment = Math.random() * 2 - 1;
        let timestamp = new gs.protos.google.protobuf.Timestamp.fromObject({seconds: i});
        let RecentSentiment = {
          datetime: timestamp,
          sentiment: sentiment
        }
        recentSentiments.push(RecentSentiment)
      }
    }
  }

  let GetRecentSentimentsReply = {
    recentSentiments
  }

  return GetRecentSentimentsReply;
}

async function GetRecentSentiments(call, callback) {
  callback(null, await GetRecentSentimentsInternal(call.request));
}

function getServer() {
  var server = new grpc.Server();
  server.addService(twitterdash.DatabaseReader.service, {
    GetCurrentTrends: GetCurrentTrends,
    GetAvailableCountries: GetAvailableCountries,
    GetRecentTrends: GetRecentTrends,
    GetTrendsWithAvailableSentiment: GetTrendsWithAvailableSentiment,
    GetCurrentSentiment: GetCurrentSentiment,
    GetRecentSentiments: GetRecentSentiments
  });
  return server;
}

if (require.main === module) {
  // If this is run as a script, start a server on an unused port
  var DatabaseReaderServer = getServer();
  DatabaseReaderServer.bindAsync(`${DB_SERVICE_HOSTNAME}:${DB_SERVICE_PORT}`, grpc.ServerCredentials.createInsecure(), () => {
    console.log("Starting DataBaseReaderServer ... ");
    DatabaseReaderServer.start();
  });
}

exports.getServer = getServer;
