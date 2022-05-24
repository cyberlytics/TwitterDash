FROM node:16.13.0-alpine3.12

RUN mkdir -p /home/app

WORKDIR /home/app

COPY package.json .

RUN npm install

COPY . .

CMD ["npm", "start"]
