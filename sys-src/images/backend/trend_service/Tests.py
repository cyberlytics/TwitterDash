import unittest
from unittest.mock import patch, Mock, MagicMock
from twitter_api_caller import Twitter_API_Caller
import json


class TestTrendService(unittest.TestCase):
    api_v2 = MagicMock()
    api_v2.get_recent_tweets_count.return_value = ["","","",{"total_tweet_count":420815}]

    api_v1 = MagicMock()
    api_v1.available_trends.return_value = json.load(open("./Test_Inputs/available_trends.json"))
    api_v1.get_place_trends.return_value = json.load(open("./Test_Inputs/get_place_trends.json"))
    

    with patch.object(Twitter_API_Caller, 'create_client_v1', return_value=api_v1):
        with patch.object(Twitter_API_Caller, 'create_client_v2', return_value=api_v2):
            caller = Twitter_API_Caller("","","","","","","","","", debug=True)

    def test_getAvailableTrends(self):
        res = {1: 'Worldwide'}
        trends = self.caller.getAvailableTrends()

        self.assertEqual(trends, res)

    def test_getTrending(self):
        res = [{'hashtag': '#PrideMonth',
                'top': 1,
                'tweet_volume': 219346,
                'timestamp': '2022-06-01T18:02:42Z',
                'country': 23424829},
                {'hashtag': '#Tankrabatt',
                'top': 2,
                'tweet_volume': 420815,
                'timestamp': '2022-06-01T18:02:42Z',
                'country': 23424829},
                {'hashtag': '#9EuroTicket',
                'top': 3,
                'tweet_volume': 420815,
                'timestamp': '2022-06-01T18:02:42Z',
                'country': 23424829}]
        trends = self.caller.getTrending()
        self.assertEqual(trends, res)
        

if __name__ == '__main__':
    unittest.main()