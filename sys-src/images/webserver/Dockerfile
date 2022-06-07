FROM node:16.15.0-alpine

WORKDIR /home/app/

COPY . .

RUN npm install

EXPOSE 3000

CMD ["npm", "run", "dev"]