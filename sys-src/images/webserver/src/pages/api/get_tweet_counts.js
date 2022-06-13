const fs = require('fs');

export default function handler(req, res) {
    let hashtag = req.query.hashtag
    let tweet_counts = null
    if (hashtag == "YetToCome") {
        let tweet_counts = require('../../dummy_data/YetToCome.json');
        res.status(200).json(tweet_counts)
    }
    else if (hashtag == "BTS_Proof") {
        let tweet_counts = require('../../dummy_data/BTS_Proof.json');
        res.status(200).json(tweet_counts)
    }
    else {
        res.status(404).json(tweet_counts)
    }
}