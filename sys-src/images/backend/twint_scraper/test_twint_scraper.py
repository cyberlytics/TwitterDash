# Imports
import unittest
from unittest.mock import patch, Mock, MagicMock
from pandas.util.testing import assert_frame_equal
from twint_scraper import Twint_Scraper
import pickle
import json


class TestTwintScraper(unittest.TestCase):
    def test_dataframe_cleanup(self):
        with open("./Test_Inputs/twint_df.p", "rb") as f:
            twint_search_result = pickle.load(f)

        with open("./Test_Inputs/twint_df_cleanup.p", "rb") as f:
            res = pickle.load(f)

        ts = Twint_Scraper()

        tweets = ts.dataframe_cleanup(twint_search_result)

        assert_frame_equal(tweets, res)

    def test_toTweet(self):
        with open("./Test_Inputs/twint_df_cleanup.p", "rb") as f:
            twint_search_result = pickle.load(f)

        res = json.load(open("./Test_Inputs/twint_df_toTweet.json", encoding="utf-8"))

        ts = Twint_Scraper()

        tweets = ts.toTweet(twint_search_result)

        self.assertEqual(tweets, res)

    def test_convertToString_MultipleLanguages(self):
        ts = Twint_Scraper()

        res = ts.convertToString(["en", "de"])

        self.assertEqual(res, " lang:en OR lang:de")

    def test_convertToString_SingleLanguages(self):
        ts = Twint_Scraper()

        res = ts.convertToString(["de"])

        self.assertEqual(res, " lang:de")


if __name__ == "__main__":
    unittest.main()
