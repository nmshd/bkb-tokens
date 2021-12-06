# Build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /src
COPY . .
RUN dotnet restore "Tokens.API/Tokens.API.csproj"

WORKDIR "/src/Tokens.API"
RUN dotnet publish --configuration Release --output /app --no-restore

# Run	
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim
EXPOSE 80
WORKDIR /app
COPY --from=build-env /app .

LABEL org.opencontainers.image.source = "https://github.com/nmshd/bkb-tokens"

ENTRYPOINT ["dotnet", "Tokens.API.dll"]
