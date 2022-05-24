# Imports 
import tweepy


class Twitter_API_Caller():
    def __init__(self, consumer_key_v1, consumer_secret_v1, access_token_v1, access_token_secret_v1):
        #v1.1
        auth_v1 = tweepy.OAuthHandler(consumer_key_v1, consumer_secret_v1)
        auth_v1.set_access_token(access_token_v1,access_token_secret_v1)
        self.api_v1 = tweepy.API(auth_v1)

        # Verfügbare Länder IDs
        self.dict_country_id = {}
        trends_loc = self.api_v1.available_trends()
        for eintrag in trends_loc:
            if eintrag["placeType"]["code"] == 12:
                self.dict_country_id[eintrag["woeid"]] = eintrag["name"]


    def getTrending(self):
        trends_for_countries = []
        
        for id in list(self.dict_country_id.keys())[:2]:
            trends = self.api_v1.get_place_trends(id)
            
            timestamp = trends[0]["as_of"]
            
            for i, trend in enumerate(trends[0]["trends"]):
                # filter topics that are no hashtags
                if trend["name"].startswith("#"):
                    name = trend["name"]
                    tweet_volume = trend["tweet_volume"]
                    trends_for_countries.append({"hashtag": name, "top": i, "tweet_volume": tweet_volume, "timestamp": timestamp, "country": id})

        # save in Database
        # TODO  