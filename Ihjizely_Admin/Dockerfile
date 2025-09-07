# Stage 1: Build with Node
FROM node:20-alpine as build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# Stage 2: Serve with Caddy
FROM caddy:2-alpine
WORKDIR /usr/share/caddy

# Copy built files from build stage
COPY --from=build /app/dist .

# Expose standard HTTP ports
EXPOSE 80
EXPOSE 443

# Note: Caddyfile is mounted from host via docker-compose