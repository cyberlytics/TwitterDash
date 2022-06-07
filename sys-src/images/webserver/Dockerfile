#FROM node:18.2.0-alpine3.15
FROM node:16.15.0-alpine
#17-stretch-slim, 17.9-stretch-slim, 17.9.0-stretch-slim
#16.15.0-alpine3.14, gallium-alpine3.14, lts-alpine3.14
#16.15.0-alpine, 16.15.0-alpine3.15, gallium-alpine, gallium-alpine3.15, lts-alpine, lts-alpine3.15

WORKDIR /home/app/

#+RUN mkdir -p /home/app/src

COPY . .

#COPY ./src/package*.json ./src

#WORKDIR src

RUN npm install

EXPOSE 3000

CMD ["npm", "run", "dev"]
#CMD ls -la src