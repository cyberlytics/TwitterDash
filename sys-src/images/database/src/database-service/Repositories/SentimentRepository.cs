using DatabaseService.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace database_service.Repositories
{
    public interface ISentimentRepository
    {
        Task StoreSentiment(IEnumerable<Sentiment> sentiments);
        Task<IEnumerable<long>> MakeIDsUnique(IEnumerable<long> tweetIDs);
    }

    public class SentimentRepository : ISentimentRepository
    {
        private IMongoCollection<Sentiment> collection;

        public SentimentRepository(IMongoCollection<Sentiment> collection)
        {
            this.collection = collection;
        }

        public async Task<IEnumerable<long>> MakeIDsUnique(IEnumerable<long> tweetIDs)
        {
            HashSet<long> querryTweets = new(tweetIDs);
            var existingTweets = await this.collection.FindAsync(x=> querryTweets.Contains(x.Tweet_ID));
            return tweetIDs.Except(existingTweets.ToList().Select(x=>x.Tweet_ID));
        }

        public async Task StoreSentiment(IEnumerable<Sentiment> sentiments)
        {
            await collection.InsertManyAsync(sentiments);
        }
    }

 
}
