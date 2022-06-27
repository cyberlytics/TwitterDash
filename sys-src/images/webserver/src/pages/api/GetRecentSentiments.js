import {DATABASE_READER_CLIENT} from "../../util/DatabaseReaderClient"
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
                res.status(200).json(data.recentSentiments);
                resolve();
            }
        }
        let GetRecentSentimentRequest = buildProtoRequest(req, ["trendName", "granularity"]);
        GetRecentSentimentRequest["start_date"] = convertToProtoTimeStamp(new Date(req.query.start_date))
        GetRecentSentimentRequest["end_date"] = convertToProtoTimeStamp(new Date(req.query.end_date));
        DATABASE_READER_CLIENT.GetRecentSentiments(GetRecentSentimentRequest, dataCallBack);
    });
}