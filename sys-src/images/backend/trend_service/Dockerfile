# from https://hub.docker.com/_/python/

FROM python:3.10

EXPOSE 50051

WORKDIR /usr/src/app

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

CMD [ "python", "./trend_service.py" ]