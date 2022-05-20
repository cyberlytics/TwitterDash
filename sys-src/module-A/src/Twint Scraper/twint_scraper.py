import twint
import pandas as pd
import nest_asyncio
from ast import literal_eval
import re


class Twint_Scraper():
    def __init__(self):#
        self.df = None
        
        nest_asyncio.apply()
     
    def extract_reply_to(self, stringliste):
        if len(stringliste) > 10:
            mentionsliste = []
            liste = literal_eval(stringliste)
            for mention in liste:
                mentionsliste.append(mention["screen_name"])
            return mentionsliste
        else:
            return stringliste
     
        
    def searchTwint(self, keyword, since=None, until=None, language=None, limit=None, createDataFrame=True, hide_out=True):
        c = twint.Config()
        c.Search = "{} lang:{}".format(keyword, language)
        c.Filter_retweets = False
        if language is not None:
            c.Lang = language
        if since is not None:
            c.Since = since
        if until is not None:
            c.Until = until
        if limit is not None:
            c.Limit = limit
        c.Hide_output = hide_out
        if createDataFrame:
            c.Pandas = True
        twint.run.Search(c)
        if createDataFrame:
            self.df = twint.storage.panda.Tweets_df
            self.df = self.dataframe_cleanup(self.df)
            return self.df
        
    def dataframe_cleanup(self, df):
        df = df.drop_duplicates(subset=["id"])
        df.drop(columns=["created_at", "place", "cashtags", "user_id_str", "day", "hour", "urls", "thumbnail", "quote_url", "near", "geo", "source", "user_rt_id", "user_rt", "retweet_id", "retweet", "retweet_date", "translate", "trans_src", "trans_dest"], inplace=True)
        df = df[df["language"] == "de"]
        df["date"] = df["date"].astype("datetime64")
        #df["created_at"] = df["created_at"].astype("datetime64")
        df["Wochentag"] = df["date"].apply(lambda x: x.weekday())
        df["Stunde"] = df["date"].apply(lambda x: x.hour)
        df["Originale_Tweetlaenge"] = df["tweet"].apply(lambda x: len(x))
        df["reply_to"] = df["reply_to"].apply(lambda x: str(self.extract_reply_to(x)))
        
        return df
        
    # Data Analysis
    # TODO  
    
    
    # Sentiment Analysis
    # TODO  
    
    
    # save in Database
    # TODO  
    
# if __name__ == "__main__":
#     twint_scraper = Twint_Scraper()
#     testdf = twint_scraper.searchTwint("corona", since="2022-05-18", until="2022-05-19", language="de", limit=500, createDataFrame=True)
#     print(testdf)