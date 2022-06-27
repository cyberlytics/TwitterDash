import {TREND_SERVICE_CLIENT} from "../../util/TrendServiceClient"
import {buildProtoRequest, convertToProtoTimeStamp} from "../../util/util";

export default async function handler(req, res) {
    return new Promise((resolve, reject) => {
        let dataCallBack = (error, data) => {
            if (error) {
                console.log("Error occured!");
                console.log(error);
                res.status(404).json({});
                reject(error);
            }
            else {
                res.status(200).json(data.tweetCounts);
                resolve();
            }
        }

        let GetRecentTweetCountsRequest = buildProtoRequest(req, ["query", "granularity"]);
        GetRecentTweetCountsRequest["start_date"] = convertToProtoTimeStamp(new Date(req.query.start_date))
        GetRecentTweetCountsRequest["end_date"] = convertToProtoTimeStamp(new Date(req.query.end_date));
        TREND_SERVICE_CLIENT.GetRecentTweetCounts(GetRecentTweetCountsRequest, dataCallBack);
    });
}