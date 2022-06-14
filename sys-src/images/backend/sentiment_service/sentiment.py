import pandas as pd
import re
from textblob_de import TextBlobDE
from transformers import AutoModelForSequenceClassification, AutoTokenizer
#from typing import List
import torch
import re
# python3 -m textblob.download_corpora
import nltk
nltk.download('punkt')


class Sentiment_Service_Blob():
    def __init__(self, onlyThree=False):
        self.onlyThreeLabels = onlyThree
        self.clean_chars = re.compile(r'[^A-Za-züöäÖÜÄß ]', re.MULTILINE)
        self.clean_http_urls = re.compile(r'https*\\S+', re.MULTILINE)
        self.clean_at_mentions = re.compile(r'@\\S+', re.MULTILINE)

    def get_sentiment(self, text, language):
        blob = TextBlobDE(self.clean_text(text))

        if self.onlyThreeLabels:
            if blob.sentiment.polarity > 0:
                return 1
            elif blob.sentiment.polarity == 0:
                return 0
            else:
                return -1
        else:
            return blob.sentiment.polarity

    def clean_text(self, text):
        text = text.replace("\n", " ")        
        text = self.clean_http_urls.sub('',text)
        text = self.clean_at_mentions.sub('',text)                     
        text = self.clean_chars.sub('', text) # use only text chars                          
        text = ' '.join(text.split()) # substitute multiple whitespace with single whitespace   
        text = text.strip().lower()
        return text


class Sentiment_Service_Transformer():
    def __init__(self):
        self.model_name = "oliverguhr/german-sentiment-bert"
        self.tokenizer = AutoTokenizer.from_pretrained(self.model_name)

        self.clean_chars = re.compile(r'[^A-Za-züöäÖÜÄß ]', re.MULTILINE)
        self.clean_http_urls = re.compile(r'https*\\S+', re.MULTILINE)
        self.clean_at_mentions = re.compile(r'@\\S+', re.MULTILINE)

    def init_model(self):
        self.model = AutoModelForSequenceClassification.from_pretrained(self.model_name, torchscript=True).to("cpu")

        dummy_example = ["Ich hasse mein Leben so sehr.", "Ich liebe mein Leben"]
        encoded = self.tokenizer.batch_encode_plus(dummy_example, padding=True, add_special_tokens=True,truncation=True, return_tensors="pt")
        self.model.eval()

        dummy_input = [encoded["input_ids"], encoded["attention_mask"]]

        traced_model = torch.jit.trace(self.model, [encoded["input_ids"], encoded["attention_mask"]])
        torch.jit.save(traced_model, "model.pt")

    def load_model(self):
        self.model = torch.jit.load("model.pt")
        self.model.eval()

    def clean_text(self, text):
        text = text.replace("\n", " ")        
        text = self.clean_http_urls.sub('',text)
        text = self.clean_at_mentions.sub('',text)                     
        text = self.clean_chars.sub('', text) # use only text chars                          
        text = ' '.join(text.split()) # substitute multiple whitespace with single whitespace   
        text = text.strip().lower()
        return text

    def get_sentiment(self, text, language):
        text = self.clean_text(text)

        encoded = self.tokenizer.batch_encode_plus([text], padding=True, add_special_tokens=True,truncation=True, return_tensors="pt")
        input_features = [encoded["input_ids"], encoded["attention_mask"]]

        output = self.model(*input_features)[0].argmax(1)

        # Klassenrange
        if (output == 0):
            return 1.0
        elif (output == 1):
            return -1.0
        else:
            return 0.0
            

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

