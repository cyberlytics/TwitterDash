﻿using DatabaseService.Models;
using DatabaseService.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Twitterdash;

namespace DatabaseService.Controller
{
    public class DatabaseReaderController : Twitterdash.DatabaseReader.DatabaseReaderBase
    {

        private woeid Woeid;
        private ITwitterTrendsRepository trendRepository;
        private readonly ISentimentRepository sentimentRepository;

        public DatabaseReaderController(ITwitterTrendsRepository trendRepository, ISentimentRepository sentimentRepository, woeid Woeid)
        {
            this.trendRepository = trendRepository;
            this.sentimentRepository = sentimentRepository;
            this.Woeid = Woeid;
        }

        public override async Task<GetAvailableCountriesReply> GetAvailableCountries(Empty request, ServerCallContext context)
        {
            var countries = await trendRepository.GetAvailableCountries();
            var getAvailableCountriesReply = new GetAvailableCountriesReply();

            foreach (int woeid in countries)
            {
                getAvailableCountriesReply.Countries.Add(this.Woeid.getCountry(woeid));
            }
            return getAvailableCountriesReply;
        }

        public override async Task<TrendProviderReply> GetCurrentTrends(GetCurrentTrendsRequest request, ServerCallContext context)
        {

            Console.WriteLine(request);

            TwitterTrends? reply = await trendRepository.GetCurrentTrends(this.Woeid.getWOEID(request.Country));

            if (reply == null)
            {
                return new TrendProviderReply();
            }

            var replyTrends = new List<Trend>();

            foreach (var trend in reply.Trends)
            {
                var T = new Trend();
                T.TrendType = (Twitterdash.TrendType)trend.trendType;
                T.Country = trend.woeid;
                T.Name = trend.name;
                T.Placement = trend.placement;
                T.TweetVolume24 = trend.tweetVolume24;

                replyTrends.Add(T);
            }

            replyTrends = replyTrends.OrderBy(x => x.Placement).ToList();
            if (request.Limit > 0)
            {
                replyTrends = replyTrends.Take(request.Limit).ToList();
            }

            var trendproviderreply = new TrendProviderReply();
            trendproviderreply.Timestamp = reply.DateTime.ToUniversalTime().ToTimestamp();
            trendproviderreply.Trends.Add(replyTrends);

            return trendproviderreply;
        }

        public override async Task<GetRecentTrendsReply> GetRecentTrends(GetRecentTrendsRequest request, ServerCallContext context)
        {
            var db_reply = await trendRepository.GetRecentTrends(
                request.StartDate?.ToDateTime(),
                request.EndDate?.ToDateTime(),
                request.Hashtag);

            var recentTrends = new List<RecentTrend>();

            foreach (var reply in db_reply)
            {
                var recentTrend = new RecentTrend();
                recentTrend.Datetime = reply.DateTime.ToUniversalTime().ToTimestamp();
                recentTrend.Trend = new();

                var found = false;
                foreach (var trend in reply.Trends)
                {
                    if (trend.name == request.Hashtag && found == false)
                    {
                        recentTrend.Trend.TrendType = (Twitterdash.TrendType)trend.trendType;
                        recentTrend.Trend.Country = trend.woeid;
                        recentTrend.Trend.Name = trend.name;
                        recentTrend.Trend.Placement = trend.placement;
                        recentTrend.Trend.TweetVolume24 = trend.tweetVolume24;
                        found = true;
                    }
                }
                recentTrends.Add(recentTrend);
            }

            var getRecentTrendsReply = new GetRecentTrendsReply();
            getRecentTrendsReply.RecentTrends.Add(recentTrends);

            return getRecentTrendsReply;
        }

        public override async Task<GetUniqueTweetsPayload> GetUniqueTweets(GetUniqueTweetsPayload request, ServerCallContext context)
        {
            var tweetIDs = request.TweetIds.ToList();
            var cleanTweetIDs = await sentimentRepository.FilterStoredIds(tweetIDs);
            var response = new GetUniqueTweetsPayload();
            response.TweetIds.Add(cleanTweetIDs);
            return response;
        }

        public override async Task<GetAvailableSentimentTrendsReply> GetAvailableSentimentTrends(GetAvailableSentimentTrendsRequest request, ServerCallContext context)
        {
            var sentiments = await sentimentRepository.GetAvailableSentimentTrends(request.Query, request.Limit);
            var trends = sentiments.Select(x => x.Trend);
            var reply = new GetAvailableSentimentTrendsReply();
            reply.AvailableTrendsWithSentiment.Add(trends);
            return reply;
        }

        public override async Task<GetCurrentSentimentReply> GetCurrentSentiment(GetCurrentSentimentRequest request, ServerCallContext context)
        {
            var sentiment = await sentimentRepository.GetCurrentSentiment(request.TrendName);
            var reply = new GetCurrentSentimentReply();
            reply.Sentiment = sentiment.Value;
            return reply;
        }

        public override async Task<GetRecentSentimentReply> GetRecentSentiment(GetRecentSentimentRequest request, ServerCallContext context)
        {
            var sentiments = await sentimentRepository.GetRecentSentiment(
                request.TrendName,
                request.StartDate?.ToDateTime(),
                request.EndDate?.ToDateTime()
            );
            var reply = new GetRecentSentimentReply();
            reply.RecentSentiments.Add(sentiments.Select(x =>
            {
                var recentSentiment = new RecentSentiment();
                recentSentiment.Datetime = x.Timestamp.ToTimestamp();
                recentSentiment.Sentiment = x.Value;
                return recentSentiment;
            }));
            return reply;
        }
    }
}