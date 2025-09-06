#Devlopment
#FROM node:20-alpine

#WORKDIR /app
#COPY package*.json ./
#RUN npm install
#COPY . .
#EXPOSE 5173
#CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]

#production
# Stage 1: Build with Node
FROM node:20-alpine as build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# Production stage - copy built files to the right location
FROM alpine:latest
RUN mkdir -p /usr/share/caddy
COPY --from=build /app/dist /usr/share/caddy