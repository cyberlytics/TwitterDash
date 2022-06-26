import {DATABASE_READER_CLIENT} from "../../util/DatabaseReaderClient"
import {buildProtoRequest} from "../../util/util";

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
                res.status(200).json(data.availableTrendsWithSentiment);
                resolve();
            }
        }
        let GetAvailableSentimentTrendsRequest = buildProtoRequest(req, ["query", "limit", "country"]);

        DATABASE_READER_CLIENT.GetAvailableSentimentTrends(GetAvailableSentimentTrendsRequest, dataCallBack);
    });
}