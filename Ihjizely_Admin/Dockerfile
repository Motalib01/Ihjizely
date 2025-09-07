# Stage 1: Build with Node
FROM node:20-alpine as build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# Stage 2: Serve with Caddy on port 80
FROM caddy:2-alpine
WORKDIR /usr/share/caddy

# Copy built files from build stage
COPY --from=build /app/dist .

# Copy custom Caddyfile
# COPY ./Caddyfile /etc/caddy/Caddyfile

# Expose standard HTTP ports
EXPOSE 80
EXPOSE 443