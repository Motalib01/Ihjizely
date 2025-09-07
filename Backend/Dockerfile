# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ihjezly.sln
RUN dotnet publish src/Ihjezly.Api/Ihjezly.Api.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Install certificates for HTTPS
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Use ports 5050 (HTTP) and 5051 (HTTPS)
EXPOSE 5050 5051

# Environment variables
ENV ASPNETCORE_URLS=http://+:5050
ENV ASPNETCORE_ENVIRONMENT=Production

# Create directory for certificates
RUN mkdir /https

ENTRYPOINT ["dotnet", "Ihjezly.Api.dll"]