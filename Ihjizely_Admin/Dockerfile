# Stage 1: Build frontend
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build   # produces /app/dist

# Stage 2: Serve with Caddy (HTTP only)
FROM caddy:alpine
COPY --from=build /app/dist /usr/share/caddy
