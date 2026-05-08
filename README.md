

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

## 📄 License
MIT

![Crossy Road Logo](static/assets/CrossyRoadLogo.png)

A modern, full-stack reimagining of the classic Crossy Road game, featuring a Svelte frontend and a .NET backend. This project is containerized for easy deployment and includes OAuth integration for authentication.

---

## Features
- Svelte frontend (Vite-powered)
- .NET backend (ASP.NET Core Web API)
- OAuth login (Bosch & Microsoft)
- Dockerized for production
- NGINX reverse proxy with custom buffer settings

---

## Getting Started

### Prerequisites
- Docker & Docker Compose
- Node.js (for local frontend dev)
- .NET 8 SDK (for local backend dev)

### Environment Variables
Copy `.env.example` to `.env` and fill in the required values:

```sh
cp .env.example .env
```

---

## Running Locally

### Backend
```sh
cd backend
# Update .env with DB and OAuth values
# Run with Docker Compose
cd ..
docker-compose up backend db
```

### Frontend
```sh
npm install
npm run dev
```

---

## Production Deployment

Use the provided `docker-compose.prod.yml` for production. This will start the frontend, backend, and database containers.

```sh
docker compose -f docker-compose.prod.yml up -d
```

---

## NGINX Configuration

To serve the frontend and proxy API requests to the backend, use the following NGINX config snippet:

```nginx
server {
	listen 80;
	server_name your-domain.example.com;

	location / {
		proxy_pass http://frontend:3000;
	}

	location /api/ {
		proxy_pass http://backend:8080/;
		proxy_set_header X-Forwarded-Host $host;
		proxy_buffer_size 256k;
		proxy_buffers 4 256k;
		proxy_busy_buffers_size 256k;
		large_client_header_buffers 4 128k;
	}
}
```

- All `/api/` requests are mapped to the backend.
- Custom buffer settings are required for large headers and payloads.

---

## API Path Mapping
- Frontend fetches API from `/api/` (see `PUBLIC_API_URL` in `.env.example`)
- NGINX proxies `/api/` to backend service

---
