# syntax=docker/dockerfile:1
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH
WORKDIR /src

COPY *.sln ./
COPY LXGaming.TorrentAnalytics/*.csproj LXGaming.TorrentAnalytics/
RUN dotnet restore LXGaming.TorrentAnalytics --arch $TARGETARCH

COPY LXGaming.TorrentAnalytics/ LXGaming.TorrentAnalytics/
RUN dotnet publish LXGaming.TorrentAnalytics --arch $TARGETARCH --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine
RUN apk add --no-cache --upgrade tzdata
WORKDIR /app
COPY --from=build /app ./
USER $APP_UID
ENTRYPOINT ["./LXGaming.TorrentAnalytics"]