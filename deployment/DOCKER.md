# 🐳 Docker - Local & Production

---

## 📦 Développement Local avec Docker Compose

### Setup Rapide

```bash
# 1. Créer .env
cp .env.example .env

# 2. Éditer .env avec vos Google OAuth credentials
# GOOGLE_CLIENT_ID=YOUR_ID
# GOOGLE_CLIENT_SECRET=YOUR_SECRET

# 3. Lancer
docker-compose up -d

# 4. Voir logs
docker-compose logs -f gentralretien

# 5. Accéder
# http://localhost:5000
```

### Ou avec le Script PowerShell

```powershell
.\scripts\DOCKER_DEPLOY.ps1 up
.\scripts\DOCKER_DEPLOY.ps1 logs
.\scripts\DOCKER_DEPLOY.ps1 down
```

### Volumes et Données

```yaml
# docker-compose.yml
volumes:
  gentralretien-db:/app/data  # ← Persistent database
```

La base de données SQLite est stockée dans un Docker volume.

---

## 🔨 Build Docker

### Dockerfile Multi-Stage

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
RUN dotnet restore
RUN dotnet publish -c Release

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
COPY --from=build /app/publish .
EXPOSE 5000 5001
CMD ["dotnet", "GEntretien.dll"]
```

### Build Manuel

```bash
# Build
docker build -t gentralretien:latest .

# Lancer
docker run -p 5000:5000 \
  -e Authentication__Google__ClientId=YOUR_ID \
  -e Authentication__Google__ClientSecret=YOUR_SECRET \
  gentralretien:latest
```

---

## 🚀 Production

### Images Registry

#### GitHub Container Registry (ghcr.io)

```bash
# Login
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Tag
docker tag gentialretien:1.0.0 ghcr.io/USERNAME/gentralretien:1.0.0

# Push
docker push ghcr.io/USERNAME/gentralretien:1.0.0

# Pull
docker pull ghcr.io/USERNAME/gentralretien:1.0.0
```

#### Docker Hub

```bash
# Login
docker login

# Tag
docker tag gentralretien:1.0.0 USERNAME/gentralretien:1.0.0

# Push
docker push USERNAME/gentralretien:1.0.0
```

---

## 🔒 Secrets Management

### ⚠️ JAMAIS passer secrets en ligne de commande

```bash
# ❌ MAUVAIS (visible dans ps aux)
docker run ... -e SECRET=myvalue ...

# ✅ BON (fichier)
docker run ... --env-file .env ...

# ✅ MEILLEUR (secrets Docker Swarm)
docker secret create my_secret value.txt
docker service create --secret my_secret ...

# ✅ MEILLEUR (Kubernetes)
kubectl create secret generic my-secret --from-literal=key=value
```

Voir [SECRETS_MANAGEMENT.md](SECRETS_MANAGEMENT.md) pour détails.

---

## 📊 Docker Compose Commandes

```bash
# Démarrer
docker-compose up -d

# Logs
docker-compose logs              # Tous les logs
docker-compose logs -f gentralretien    # Logs gentralretien
docker-compose logs --tail 100   # Dernières 100 lignes

# Status
docker-compose ps

# Shell dans container
docker-compose exec gentralretien bash

# Arrêter
docker-compose down

# Arrêter + supprimer volumes
docker-compose down -v

# Rebuild
docker-compose up -d --build
```

---

## 🏗️ Production Setup

### docker-compose.prod.yml

```yaml
version: '3.8'

services:
  gentralretien:
    image: ghcr.io/yourusername/gentralretien:1.0.0
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000
      - ConnectionStrings__Default=Data Source=/app/data/app.db
      # Secrets via env (ou Docker Secrets, voir SECRETS_MANAGEMENT.md)
      - Authentication__Google__ClientId=${GOOGLE_CLIENT_ID}
      - Authentication__Google__ClientSecret=${GOOGLE_CLIENT_SECRET}
    volumes:
      - /data/gentralretien:/app/data
    restart: always
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3

networks:
  default:
    name: prod-network
    driver: bridge
```

Déployer:
```bash
docker-compose -f docker-compose.prod.yml up -d
```

---

## 🔐 Environment Variables

```env
# Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000

# Database
ConnectionStrings__Default=Data Source=/app/data/app.db

# Google OAuth
Authentication__Google__ClientId=YOUR_ID
Authentication__Google__ClientSecret=YOUR_SECRET

# Logging
Logging__LogLevel__Default=Information
```

Lire depuis:
- `.env` file
- Docker Secrets (Swarm/K8s)
- Environment variables
- appsettings.json (pour defaults)

---

## ✅ Health Checks

```csharp
// Ajouter endpoint dans Program.cs
app.MapHealthChecks("/health");
```

```bash
# Vérifier
curl http://localhost:5000/health
```

```yaml
# Dans docker-compose.yml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
  interval: 30s
  timeout: 10s
  retries: 3
  start_period: 40s
```

---

## 🔗 Networking

### Bridge Network (Default)
```bash
# Les containers peuvent se parler par nom
docker network ls
docker network inspect bridge
```

### Custom Network
```yaml
# docker-compose.yml
networks:
  gentralretien-network:
    driver: bridge
```

### Expose Ports
```yaml
ports:
  - "5000:5000"  # HOST:CONTAINER
```

---

## 📈 Monitoring

### Logs
```bash
docker logs gentralretien
docker logs --follow gentralretien
docker logs --tail 50 gentralretien
```

### Stats
```bash
docker stats gentralretien
# Affiche: CPU, Memory, Network I/O
```

### Events
```bash
docker events --filter container=gentralretien
```

---

## 🎯 Best Practices

- ✅ Use `.env` files (in `.gitignore`)
- ✅ Use volumes pour persistent data
- ✅ Health checks pour auto-restart
- ✅ Resource limits (CPU, Memory)
- ✅ Logging à stdout (pas fichiers)
- ✅ Non-root user (security)
- ✅ Multi-stage builds (taille)

---

## 🚀 CI/CD

GitHub Actions build + push (sample):
```yaml
- name: Build & Push
  run: |
    docker build -t ghcr.io/${{ github.repository }}:${{ github.sha }} .
    echo ${{ secrets.GITHUB_TOKEN }} | docker login ghcr.io -u ${{ github.actor }} --password-stdin
    docker push ghcr.io/${{ github.repository }}:${{ github.sha }}
```

---

Voir [SECRETS_MANAGEMENT.md](SECRETS_MANAGEMENT.md) pour production security.
