# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src
COPY . .
RUN dotnet restore "Tokens.API/Tokens.API.csproj"

WORKDIR "/src/Tokens.API"
RUN dotnet publish --configuration Release --output /app --no-restore

# Run	
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
EXPOSE 80
WORKDIR /app
COPY --from=build-env /app .

LABEL org.opencontainers.image.source = "https://github.com/nmshd/bkb-tokens"

ENTRYPOINT ["dotnet", "Tokens.API.dll"]
