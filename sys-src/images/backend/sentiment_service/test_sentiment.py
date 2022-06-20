import unittest
from unittest.mock import patch, Mock, MagicMock
from pandas.util.testing import assert_frame_equal

from sentiment import Sentiment_Service_Blob, Sentiment_Service_Transformer


class TestSentimentService(unittest.TestCase):

    blob = Sentiment_Service_Blob()
    transformer = Sentiment_Service_Transformer()

    # Test the sentiment service with TextBlobDE
    def test_Blob_clean_text(self):
        self.assertEqual(self.blob.clean_text("test"), "test")

    def test_Blob_clean_text_with_upper_case(self):
        self.assertEqual(self.blob.clean_text("test TEST"), "test test")

    def test_Blob_clean_text_with_numbers(self):
        self.assertEqual(self.blob.clean_text("test 1"), "test")

    def test_Blob_clean_text_with_numbers_and_special_characters(
        self,
    ):
        self.assertEqual(
            self.blob.clean_text("test test test 1 !@#$%^&*()_+"), "test test test"
        )

    def test_Blob_clean_text_with_https(self):
        self.assertEqual(
            self.blob.clean_text("test test https://www.oth-aw.de"), "test test"
        )

    def test_Blob_clean_text_with_mention(self):
        self.assertEqual(self.blob.clean_text("test test @BastianHahn6"), "test test")

    def test_Blob_clean_text_with_more_whitespaces(self):
        self.assertEqual(self.blob.clean_text("test  test    test"), "test test test")

    def test_Blob_clean_text_with_newlines(self):
        self.assertEqual(self.blob.clean_text("test\ntest"), "test test")

    def test_Blob_clean_text_with_german_umlaute(self):
        self.assertEqual(self.blob.clean_text("test äöüß"), "test äöüß")

    # Test the sentiment service with Transformer model
    def test_Transformer_clean_text(self):
        self.assertEqual(self.transformer.clean_text("test"), "test")

    def test_Transformer_clean_text_with_upper_case(self):
        self.assertEqual(self.transformer.clean_text("test TEST"), "test test")

    def test_Transformer_clean_text_with_numbers(self):
        self.assertEqual(self.transformer.clean_text("test 1"), "test")

    def test_Transformer_clean_text_with_numbers_and_special_characters(
        self,
    ):
        self.assertEqual(
            self.transformer.clean_text("test test test 1 !@#$%^&*()_+"),
            "test test test",
        )

    def test_Transformer_clean_text_with_https(self):
        self.assertEqual(
            self.transformer.clean_text("test test https://www.oth-aw.de"), "test test"
        )

    def test_Transformer_clean_text_with_mention(self):
        self.assertEqual(
            self.transformer.clean_text("test test @BastianHahn6"), "test test"
        )

    def test_Transformer_clean_text_with_more_whitespaces(self):
        self.assertEqual(
            self.transformer.clean_text("test  test    test"), "test test test"
        )

    def test_Transformer_clean_text_with_newlines(self):
        self.assertEqual(self.transformer.clean_text("test\ntest"), "test test")

    def test_Transformer_clean_text_with_german_umlaute(self):
        self.assertEqual(self.transformer.clean_text("test äöüß"), "test äöüß")


if __name__ == "__main__":
    unittest.main()
