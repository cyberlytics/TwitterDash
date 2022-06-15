# from https://hub.docker.com/_/python/

FROM python:3.10

EXPOSE 50250

WORKDIR /usr/src/app

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

WORKDIR /usr/src/app/

RUN python -m nltk.downloader punkt

COPY . .

RUN [ "python", "./sentiment.py" ]

CMD [ "python", "./sentiment_service.py" ]