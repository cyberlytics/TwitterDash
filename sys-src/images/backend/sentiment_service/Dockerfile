# from https://hub.docker.com/_/python/

FROM python:3.10

EXPOSE 50050

WORKDIR /usr/src/app

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

WORKDIR /usr/src/app/

COPY . .

CMD [ "python", "./sentiment_service.py" ]