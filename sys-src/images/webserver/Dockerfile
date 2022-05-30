FROM node:18.2.0-alpine3.15

WORKDIR /home/app

COPY ./src/ .

WORKDIR src

RUN npm install

CMD ["npm", "start", "dev"]
