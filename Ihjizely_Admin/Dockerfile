# Stage 1: Build frontend
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy source code and build
COPY . .
RUN npm run build   # creates /app/dist

# Stage 2: Serve frontend with Caddy
FROM caddy:alpine
COPY --from=build /app/dist /usr/share/caddy
