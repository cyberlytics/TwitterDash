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
                res.status(200).json(data.recentTrends);
                resolve();
            }
        }
        let GetRecentTrendsRequest = buildProtoRequest(req, ["hashtag", "country"]);
        GetRecentTrendsRequest["start_date"] = convertToProtoTimeStamp(new Date(req.query.start_date))
        GetRecentTrendsRequest["end_date"] = convertToProtoTimeStamp(new Date(req.query.end_date));
        DATABASE_READER_CLIENT.GetRecentTrends(GetRecentTrendsRequest, dataCallBack);
    });
}