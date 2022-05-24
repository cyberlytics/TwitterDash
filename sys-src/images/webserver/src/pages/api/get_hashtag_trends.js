// Next.js API route support: https://nextjs.org/docs/api-routes/introduction
const fs = require('fs');

export default function handler(req, res) {
  let hashtag_trends = require('../../dummy_data/improved_hashtags.json');
  res.status(200).json(hashtag_trends)
}
