# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose both HTTP and HTTPS
EXPOSE 5050 5051

# Tell ASP.NET to listen on both
ENV ASPNETCORE_URLS="http://+:5050;https://+:5051"
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Ihjezly.Api.dll"]
