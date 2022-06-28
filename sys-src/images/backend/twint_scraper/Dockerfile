# from https://hub.docker.com/_/python/

FROM python:3.10

EXPOSE 50150

WORKDIR /usr/src/app

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

RUN git clone --depth=1 https://github.com/twintproject/twint.git

WORKDIR /usr/src/app/twint
RUN pip3 install .


WORKDIR /usr/src/app/

COPY . .

CMD [ "python", "./twint_service.py" ]