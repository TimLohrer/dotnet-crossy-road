

# 🐔 Crossy Road

![Crossy Road Logo](img/image.png)

---

## 🚀 Features

- Seed based map generation
- Live Multiplayer
- Leaderboard
- Skins

---

## 🛠️ Getting Started

### Prerequisites
- Docker & Docker Compose
- bun (for local frontend dev)
- .NET 10 SDK (for local backend dev)

## 🚢 Production Deployment

Ready to go live? Use the provided `docker-compose.prod.yml` to launch everything in production mode:

Copy `.env.example` to `.env` and fill in the required values:

```sh
cp .env.example .env
```

```sh
docker compose -f docker-compose.prod.yml up -d
```

---

## 🦄 NGINX Configuration

To serve the frontend and send all `/api/` requests to the backend, use this NGINX config magic:

```nginx
server {
	listen 80; # Your http port
	server_name your-domain.example.com;

	location / {
		proxy_pass http://frontend:3000;
	}

	location /api/ {
		proxy_pass http://backend:8080/;
        # The following config is required for oauth to work properly o.o
		proxy_set_header X-Forwarded-Host $host;
		proxy_buffer_size 256k;
		proxy_buffers 4 256k;
		proxy_busy_buffers_size 256k;
		large_client_header_buffers 4 128k;
	}
}
```
---