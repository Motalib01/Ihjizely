# Stage 1: Build frontend
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build   # creates /app/dist

# Stage 2: Serve files via nginx-lightweight (optional, or just use main Caddy)
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
