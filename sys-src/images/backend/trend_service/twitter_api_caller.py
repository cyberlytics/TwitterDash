# Imports
import tweepy
import datetime


class Twitter_API_Caller:
    def __init__(
        self,
        consumer_key_v1,
        consumer_secret_v1,
        access_token_v1,
        access_token_secret_v1,
        consumer_key_v2,
        consumer_secret_v2,
        access_token_v2,
        access_token_secret_v2,
        bearer_token_v2,
        debug=False,
    ):
        # v1.1
        self.api_v1 = self.create_client_v1(
            consumer_key_v1,
            consumer_secret_v1,
            access_token_v1,
            access_token_secret_v1,
        )
        # v2
        self.api_v2 = self.create_client_v2(
            consumer_key_v2,
            consumer_secret_v2,
            access_token_v2,
            access_token_secret_v2,
            bearer_token_v2,
        )

        self.debug = debug

        # Verfügbare Länder IDs
        self.dict_country_id = self.getAvailableTrends()
    
    def create_client_v1(self, consumer_key_v1, consumer_secret_v1, access_token_v1, access_token_secret_v1):
        """
        Create a new client for Twitter API v1.1

        Args:
            consumer_key_v1 (string): Consumer Key
            consumer_secret_v1 (string): Consumer Secret
            access_token_v1 (string): Access Token
            access_token_secret_v1 (string): Access Token Secret

        Returns:
            tweepy.API: Twitter API v1.1
        """        
        auth_v1 = tweepy.OAuthHandler(consumer_key_v1, consumer_secret_v1)
        auth_v1.set_access_token(access_token_v1, access_token_secret_v1)
        return tweepy.API(auth_v1, wait_on_rate_limit=True)

    def create_client_v2(
        self, consumer_key_v2, consumer_secret_v2, access_token_v2, access_token_secret_v2, bearer_token_v2
    ):
        """
        Create a new client for Twitter API v2

        Args:
            consumer_key_v2 (string): Consumer Key
            consumer_secret_v2 (string): Consumer Secret
            access_token_v2 (string): Access Token
            access_token_secret_v2 (string): Access Token Secret
            bearer_token_v2 (string): Bearer Token

        Returns:
            tweepy.API: Twitter API v2
        """        
        auth_v2 = tweepy.Client(
            consumer_key=consumer_key_v2,
            consumer_secret=consumer_secret_v2,
            access_token=access_token_v2,
            access_token_secret=access_token_secret_v2,
            bearer_token=bearer_token_v2,
            wait_on_rate_limit=True,
        )
        return auth_v2


    def getAvailableTrends(self):
        """
        Get avaiable trend countries.
        Uses Twitter API v1.1

        Returns:
            dict: dict of available trend countries
        """        
        dict_country_id = {}
        trends_loc = self.api_v1.available_trends()
        for eintrag in trends_loc:
            if eintrag["placeType"]["code"] == 12:
                dict_country_id[eintrag["woeid"]] = eintrag["name"]
        return dict_country_id
       
    def getTrending(self):
        trends_for_countries = []

        # for debugging only use WOEID for Germany
        if self.debug:
            self.dict_country_id = {23424829: "Germany"}
            # Wenn Sie das hier Lesen, bekommen Sie ein Snickers von mir (Bastian Hahn)

        for id in list(self.dict_country_id.keys()):
            trends = self.api_v1.get_place_trends(id)

            timestamp = trends[0]["as_of"]

            for i, trend in enumerate(trends[0]["trends"]):
                # filter topics that are no hashtags
                if trend["name"].startswith("#"):
                    name = trend["name"]
                    tweet_volume = trend["tweet_volume"]

                    if tweet_volume == None:
                        # tweet_volume = -1
                        # get tweet count in last 24 hours
                        tweet_volume = self.getTweetCount(
                            trend["name"],
                            "day",
                            (datetime.datetime.now() - datetime.timedelta(days=1)),
                        )
                    trends_for_countries.append(
                        {
                            "hashtag": name,
                            "top": i + 1,
                            "tweet_volume": tweet_volume,
                            "timestamp": timestamp,
                            "country": id,
                        }
                    )

        return trends_for_countries
                
                
    def getTweetCount(self, trend, granularity, start_time):
        """
        Gets tweet count for one trend, uses Twitter API v2

        Args:
            trend (string): Trend bzw. Hashtag, der nachgezählt werden soll
            granularity (string): Tag, Woche, Monat, Jahr
            start_time (datetime): Zeitpunkt, von wann die Abfrage des Tweetcounts starten soll

        Returns:
            int: Tweetcount für einen Trend
        """     
           
        tweetCount = self.api_v2.get_recent_tweets_count(
            query=trend,
            granularity=granularity,
            start_time=start_time,
        )

        return tweetCount[3]["total_tweet_count"]
