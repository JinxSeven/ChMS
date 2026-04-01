# BASE IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# BUILD STAGE
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["api/api.csproj", "api/"]
RUN dotnet restore "./api/api.csproj"

COPY . .
WORKDIR "/src/api"
RUN dotnet build "./api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH STAGE
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL STAGE
FROM base AS final

# ✅ define UID properly
ARG APP_UID=1000

# install dependency as root
USER root
RUN apt-get update && \
    apt-get install -y libkrb5-3 curl && \
    rm -rf /var/lib/apt/lists/*

# switch to non-root
USER ${APP_UID}

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "api.dll"]
