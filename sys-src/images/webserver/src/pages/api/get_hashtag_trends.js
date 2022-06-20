import {DATABASE_READER_CLIENT} from "../../util/DatabaseReaderClient"

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
                res.status(200).json(data.trends);
                resolve();
            }
        }
        let num_results = req.query.num_results;
        DATABASE_READER_CLIENT.GetCurrentTrends({limit: num_results}, dataCallBack);
    });


    //client.GetRecentTrends(RecentTrendsRequest, dataCallback);
    //client.GetAvailableCountries(null, dataCallback);
    //TODO: return data
}
