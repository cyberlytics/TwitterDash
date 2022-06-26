import {TREND_SERVICE_CLIENT} from "../../util/TrendServiceClient"

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
        let hashtag = req.query.hashtag;
        TREND_SERVICE_CLIENT.GetRecentTweetCounts({query: hashtag, country:req.query.country}, dataCallBack);
    });
}