import unittest
from unittest.mock import patch
from trend_service import TrendService

class TestTrendService(unittest.TestCase):
    def test_get_trend_data(self):
        trend_service = TrendService()
        trend_data = trend_service.get_trend_data()
        self.assertEqual(trend_data, [{'name': 'test', 'value': 0}])        # assertEqual, assertTrue, assertFalse, assertRaises, ...


        # https://stackoverflow.com/questions/37039512/mocking-twitters-api-library-with-pythons-patch-decorator

if __name__ == '__main__':
    unittest.main()