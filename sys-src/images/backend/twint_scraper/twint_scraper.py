# Imports
import twint
import pandas as pd
import nest_asyncio


class Twint_Scraper:
    def __init__(self):
        self.df = None
        nest_asyncio.apply()

    def searchTwint(
        self,
        keyword,
        since=None,
        until=None,
        language=None,
        limit=None,
        createDataFrame=True,
        hide_out=True,
    ):
        """
        Searches for tweets with the given parameters

        Args:
            keyword (string): keywoard, which is searched for
            since (Timestamp): period boundary (since)
            until (Timestamp):period boundary (until)
            language (List): language of the tweets (e.g. ["de"])
            limit (int): maximum quantity of tweets to return
            createDataFrame (bool): Creates a dataframe
            hide_out (bool): hide output in console

        Returns:
            List: emtpy list if no tweets were farmed, else a list of tweets containing the tweet's attributes
        """

        c = twint.Config()
        if language is not None:
            stringLanguage = self.convertToString(
                language
            )  # convert language list to string
            # twint c.lang has a bug, so now it is within the query
            c.Search = "{}{}".format(keyword, stringLanguage)
        else:
            c.Search = "{}".format(keyword)
        c.Filter_retweets = False
        if since is not None:
            c.Since = since
        if until is not None:
            c.Until = until
        if limit is not None:
            c.Limit = limit
        c.Hide_output = hide_out
        if createDataFrame:
            c.Pandas = True

        # Search is performed
        twint.run.Search(c)
        if createDataFrame:
            self.df = twint.storage.panda.Tweets_df

            # If no tweet was farmed for a specific query
            if self.df.empty:
                return []
            else:
                self.df = self.dataframe_cleanup(self.df)
                return self.toTweet(self.df)

    def convertToString(self, listOfLanguages):
        """
        Converts list of languages to a concatenated string, which the twitter search query can handle

        Args:
            tmpliste (list): list of languages (e.g. ["de", "en"])

        Returns:
            string: string of languages (e.g. " lang:de OR lang:en")
        """

        tmperg = ""
        lentmp = len(listOfLanguages)

        # If only one language is in the list
        if lentmp == 1:
            return f" lang:{listOfLanguages[0]}"
        else:
            # If multiple languages are in the list
            for i, eintrag in enumerate(listOfLanguages):
                if i < lentmp - 1:
                    tmperg += f" lang:{eintrag} OR"
                else:
                    # If it is the last element in the list
                    tmperg += f" lang:{eintrag}"
        return tmperg

    def dataframe_cleanup(self, df):
        """
        Cleans up the stock dataframe of the twint scraper

        Args:
            df (dataframe): dataframe of the twint scraper

        Returns:
            dataframe: cleaned dataframe
        """

        df = df.drop_duplicates(subset=["id"])  # remove duplicates

        # Drop columns which are not needed
        df.drop(
            columns=[
                "created_at",
                "place",
                "cashtags",
                "user_id_str",
                "day",
                "hour",
                "urls",
                "thumbnail",
                "quote_url",
                "near",
                "geo",
                "source",
                "user_rt_id",
                "user_rt",
                "retweet_id",
                "retweet",
                "retweet_date",
                "translate",
                "trans_src",
                "trans_dest",
            ],
            inplace=True,
        )

        # Change datatypes of specific columns
        df["date"] = df["date"].astype("datetime64")
        df["language"] = df["language"].astype(str)
        df["date"] = df["date"].astype(str)

        # Feature Engineering
        # df["Wochentag"] = df["date"].apply(lambda x: x.weekday())
        # df["Stunde"] = df["date"].apply(lambda x: x.hour)
        # df["Originale_Tweetlaenge"] = df["tweet"].apply(lambda x: len(x))

        return df

    def toTweet(self, df):
        """
        Converts the dataframe to a list of tweets, which is in the appropriate format for the gRPC service

        Args:
            df (dataframe): cleaned dataframe

        Returns:
            List: list of tweets, each containing a dictonary with the tweet's attributes
        """

        tweets = []
        for index, row in df.iterrows():  # iterate over dataframe
            tweets.append(
                {
                    "id": int(row["id"]),
                    "conversation_id": int(row["conversation_id"]),
                    "date": row["date"],
                    "tweet": row["tweet"],
                    "user_id": row["user_id"],
                    "nlikes": row["nlikes"],
                    "nreplies": row["nreplies"],
                    "nretweets": row["nretweets"],
                    "hashtags": row["hashtags"],
                    "language": row["language"],
                }
            )
        return tweets


# if __name__ == "__main__":
#     twint_scraper = Twint_Scraper()
#     testdf = twint_scraper.searchTwint("corona", since="2022-05-18", until="2022-05-19", language="de", limit=500, createDataFrame=True)
#     print(testdf)
