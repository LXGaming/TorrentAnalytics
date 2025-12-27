# TorrentAnalytics

[![License](https://img.shields.io/github/license/LXGaming/TorrentAnalytics?label=License&cacheSeconds=86400)](https://github.com/LXGaming/TorrentAnalytics/blob/main/LICENSE)
[![Docker Hub](https://img.shields.io/docker/v/lxgaming/torrentanalytics/latest?label=Docker%20Hub)](https://hub.docker.com/r/lxgaming/torrentanalytics)

## Prerequisites
- [Flood](https://flood.js.org/)
- [Grafana](https://grafana.com/)
- [InfluxDB](https://www.influxdata.com/)

## Usage
### Docker Compose
Download and use [config.json](https://raw.githubusercontent.com/LXGaming/TorrentAnalytics/main/LXGaming.TorrentAnalytics/config.json)
```yaml
services:
  torrentanalytics:
    container_name: torrentanalytics
    image: lxgaming/torrentanalytics:latest
    restart: unless-stopped
    volumes:
      - /path/to/torrentanalytics/logs:/app/logs
      - /path/to/torrentanalytics/config.json:/app/config.json
```

## License
TorrentAnalytics is licensed under the [Apache 2.0](https://github.com/LXGaming/TorrentAnalytics/blob/main/LICENSE) license.