# Imports
import pandas as pd
import re
from textblob_de import TextBlobDE
from textblob_fr import PatternTagger as PatternTaggerFR
from textblob_fr import PatternAnalyzer as PatternAnalyzerFR
from textblob_nl import PatternTagger as PatternTaggerNL
from textblob_nl import PatternAnalyzer as PatternAnalyzerNL
from textblob import TextBlob
from transformers import AutoModelForSequenceClassification, AutoTokenizer
from numpy import interp
import torch
import re
import nltk

# Sentiment Analysis with Textblob
class Sentiment_Service_Blob:
    def __init__(self, onlyThree=False):
        self.onlyThreeLabels = onlyThree

        # Regex
        self.clean_chars = re.compile(r"[^A-Za-züöäÖÜÄß ]", re.MULTILINE)
        self.clean_http_urls = re.compile(r"http\S+", re.MULTILINE)
        self.clean_at_mentions = re.compile(r"@\S+", re.MULTILINE)

    def get_sentiment(self, text, language):
        """
        Calculates a sentiment score for a specific tweet

        Args:
            text (string): raw tweet
            language (_type_): language of the tweet (en, de, nl, fr)

        Raises:
            ValueError: if language is not supported

        Returns:
            float: in range [-1, 1], or if onlyThreeLabels is true -1 = negative, 0 = neutral, 1 = positive
        """
        sentiment_value = 0.0

        # German
        if language == "de":
            blob = TextBlobDE(self.clean_text(text))
            sentiment_value = blob.sentiment.polarity

        # English
        elif language == "en":
            blob = TextBlob(self.clean_text(text))
            sentiment_value = blob.sentiment.polarity

        # French
        elif language == "fr":
            blob = TextBlob(
                self.clean_text(text),
                pos_tagger=PatternTaggerFR(),
                analyzer=PatternAnalyzerFR(),
            )
            sentiment_value = blob.sentiment[0]

        # Dutch
        elif language == "nl":
            blob = TextBlob(
                self.clean_text(text),
                pos_tagger=PatternTaggerNL(),
                analyzer=PatternAnalyzerNL(),
            )
            sentiment_value = blob.sentiment[0]

        else:
            raise ValueError("Language not supported")

        # If only three values are needed, return -1, 0, 1
        if self.onlyThreeLabels:
            if sentiment_value > 0:
                return 1
            elif sentiment_value == 0:
                return 0
            else:
                return -1
        else:
            return sentiment_value

    def clean_text(self, text):
        """
        Light cleaning of the tweet before sentiment analysis

        Args:
            text (string): raw string of the tweet

        Returns:
            string: cleaed string of the tweet
        """

        text = text.replace("\n", " ")
        text = self.clean_http_urls.sub("", text)
        text = self.clean_at_mentions.sub("", text)
        text = self.clean_chars.sub("", text)  # use only text chars
        text = " ".join(
            text.split()
        )  # substitute multiple whitespace with single whitespace
        text = text.strip().lower()
        return text


# Sentiment Analysis with a Transformer (BERT)
class Sentiment_Service_Transformer:
    def __init__(self, internationalModel=True):
        self.internationalModel = internationalModel

        # Two models are available, one for international tweets (en, nl, de, fr, it, es) and one for German tweets
        if self.internationalModel:
            self.model_name = "nlptown/bert-base-multilingual-uncased-sentiment"
        else:
            self.model_name = "oliverguhr/german-sentiment-bert"

        # Tokenizer for the model
        self.tokenizer = AutoTokenizer.from_pretrained(self.model_name)

        # Regex
        self.clean_chars = re.compile(r"[^A-Za-züöäÖÜÄß ]", re.MULTILINE)
        self.clean_http_urls = re.compile(r"http\S+", re.MULTILINE)
        self.clean_at_mentions = re.compile(r"@\S+", re.MULTILINE)

    def init_model(self):
        """
        Initializes the model and export it to torchscript (makes it run a little faster)
        """

        # Load model from huggingface
        self.model = AutoModelForSequenceClassification.from_pretrained(
            self.model_name, torchscript=True
        ).to("cpu")

        # Torchscripts exptects example inputs
        dummy_example = ["Ich hasse mein Leben so sehr.", "Ich liebe mein Leben"]

        # Tokenize the dummy example
        encoded = self.tokenizer.batch_encode_plus(
            dummy_example,
            padding=True,
            add_special_tokens=True,
            truncation=True,
            return_tensors="pt",
        )
        self.model.eval()

        dummy_input = [encoded["input_ids"], encoded["attention_mask"]]

        # Save
        traced_model = torch.jit.trace(self.model, dummy_input)
        torch.jit.save(traced_model, "model.pt")

    def load_model(self):
        """
        Loads the model from torchscript
        """

        self.model = torch.jit.load("model.pt")
        self.model.eval()

    def clean_text(self, text):
        """
        Light cleaning of the tweet before sentiment analysis

        Args:
            text (string): raw string of the tweet

        Returns:
            string: cleaed string of the tweet
        """

        text = text.replace("\n", " ")
        text = self.clean_http_urls.sub("", text)
        text = self.clean_at_mentions.sub("", text)
        text = self.clean_chars.sub("", text)  # use only text chars
        text = " ".join(
            text.split()
        )  # substitute multiple whitespace with single whitespace
        text = text.strip().lower()
        return text

    def get_sentiment(self, text, language):
        """
        Calculates a sentiment score for a specific tweet

        Args:
            text (string): raw tweet
            language (_type_): Ignored in this method

        Returns:
            float: in range [-1, 1], or if onlyThreeLabels is true -1 = negative, 0 = neutral, 1 = positive
        """

        # Clean text
        text = self.clean_text(text)

        # Tokenize text
        encoded = self.tokenizer.batch_encode_plus(
            [text],
            padding=True,
            add_special_tokens=True,
            truncation=True,
            return_tensors="pt",
        )
        input_features = [encoded["input_ids"], encoded["attention_mask"]]

        # Run it through the model
        output = self.model(*input_features)[0].argmax(1)

        # Distinction between the two models
        if self.internationalModel == False:
            if output == 0:
                return 1.0
            elif output == 1:
                return -1.0
            else:
                return 0.0
        else:
            # map range 0 to 4 to range -1 to 1
            return interp(output.item(), [0, 4], [-1, 1])


if __name__ == "__main__":
    sentiment_service = Sentiment_Service_Transformer()
    sentiment_service.init_model()


# if __name__ == "__main__":
#     # sentiment_service = Sentiment_Service_Blob(onlyThree=True)
#     # print(sentiment_service.get_sentiment("Ich hasse dieses neue Auto!"))

#     sentiment_service = Sentiment_Service_Transformer()
#     #sentiment_service.init_model()
#     sentiment_service.load_model()
#     #print(sentiment_service.predictSentiment("Ich hasse dieses neue Auto!"))

#     import json
#     testtweets = json.load(open("../../../../res/tweet_example.json", encoding="utf8"))
#     listofTweets = [tweet["Text"] for tweet in testtweets]


#     tmpliste = []
#     for tweet in tqdm.tqdm(listofTweets):
#         #print(sentiment_service.predictSentiment(tweet))
#         tmpliste.append(sentiment_service.predictSentiment(tweet))

#     print(tmpliste)
