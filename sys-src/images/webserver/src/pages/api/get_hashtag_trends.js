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
        let limit = req.query.num_results;
        let country = req.query.country;
        DATABASE_READER_CLIENT.GetCurrentTrends({limit, country}, dataCallBack);
    });
}
