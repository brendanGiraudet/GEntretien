# Docker & Deployment Guide

## Local Development with Docker

### Prerequisites
- Docker and Docker Compose installed
- .NET 10 SDK (for local development without Docker)

### Building the Docker Image Locally

```bash
# Build the image
docker build -t gentralretien:latest .

# Run the container
docker run -p 5000:5000 -p 5001:5001 \
  -e ConnectionStrings__Default="Data Source=/app/data/app.db" \
  -v gentralretien-db:/app/data \
  gentralretien:latest
```

### Using Docker Compose

The easiest way to run locally with Docker:

```bash
# Build and start the service
docker-compose up -d

# View logs
docker-compose logs -f gentralretien

# Stop the service
docker-compose down
```

Access the application at: `http://localhost:5000`

## Building for Production

### Multi-Stage Build Benefits
- **Stage 1 (Build):** Compiles and publishes .NET application
- **Stage 2 (Runtime):** Minimal runtime image with only necessary dependencies
- **Result:** Smaller image size (~200-300 MB vs 1+ GB with SDK)

### Image Optimization Tips

1. **Use `.dockerignore`** to exclude unnecessary files
2. **Multi-stage builds** to keep final image minimal
3. **Cache layers** for faster rebuilds
4. **Security:** Run as non-root user (can be added to Dockerfile)

### Adding a Non-Root User (Security Best Practice)

Update Dockerfile runtime stage:

```dockerfile
# Create non-root user
RUN useradd -m -u 1000 appuser
USER appuser
```

## GitHub Actions CI/CD Pipeline

The workflow (`.github/workflows/docker-ci.yml`) includes:

1. **Trigger Events:**
   - Push to `main` or `develop` branches
   - Pull requests to `main` or `develop`

2. **Build & Test Job:**
   - Restore dependencies
   - Build project (Release config)
   - Run unit tests
   - Upload test results as artifacts

3. **Docker Build & Push Job:**
   - Depends on `build-and-test` passing
   - Builds Docker image using Buildx
   - Pushes to GitHub Container Registry (ghcr.io)
   - Uses semantic versioning and branch tags
   - Implements layer caching for speed

### Tagging Strategy

Images are tagged with:
- `branch-<sha>` — commit SHA for traceability
- `<branch-name>` — current branch
- `latest` — only for main branch pushes
- `<version>` — for semantic version tags (future)

### Setting Up GitHub Actions

1. **Enable GitHub Actions** in repository settings
2. **No additional secrets needed** by default (uses GITHUB_TOKEN)
3. **Container Registry Access:**
   - Automatically uses GITHUB_TOKEN for authentication
   - Images push to `ghcr.io/${github-username}/gentralretien`

### Viewing Build Results

1. Go to repository → **Actions** tab
2. Select workflow run
3. View build logs and test results
4. Check pushed images: `ghcr.io/${github-username}/gentralretien`

## Deploying to Production

### Option 1: Kubernetes (Recommended)

```yaml
# Example deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gentralretien
spec:
  replicas: 2
  selector:
    matchLabels:
      app: gentralretien
  template:
    metadata:
      labels:
        app: gentralretien
    spec:
      containers:
      - name: gentralretien
        image: ghcr.io/your-org/gentralretien:latest
        ports:
        - containerPort: 5000
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__Default
          valueFrom:
            secretKeyRef:
              name: db-secret
              key: connection-string
        volumeMounts:
        - name: db-storage
          mountPath: /app/data
      volumes:
      - name: db-storage
        persistentVolumeClaim:
          claimName: gentralretien-pvc
```

### Option 2: Docker Swarm

```bash
docker service create \
  --name gentralretien \
  --replicas 2 \
  -p 5000:5000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Default="Data Source=/app/data/app.db" \
  ghcr.io/your-org/gentralretien:latest
```

### Option 3: Traditional VM/Server

```bash
# Pull and run
docker pull ghcr.io/your-org/gentralretien:latest
docker run -d \
  --name gentralretien \
  -p 80:5000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -v gentralretien-db:/app/data \
  ghcr.io/your-org/gentralretien:latest
```

## Database Persistence

### SQLite in Docker

The Dockerfile creates `/app/data` for SQLite. Mount it as a volume:

```bash
docker run -v gentralretien-db:/app/data ghcr.io/your-org/gentralretien:latest
```

### For Production: Consider PostgreSQL/MySQL

Update connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=postgres-host;Database=gentralretien;User=user;Password=pwd"
  }
}
```

## Monitoring & Logs

```bash
# View container logs
docker logs -f <container-id>

# Health check status
docker ps | grep gentralretien

# Access running container
docker exec -it <container-id> /bin/bash
```

## Troubleshooting

### Image Won't Build
- Check SDK version compatibility: `FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build`
- Verify `GEntretien.csproj` path in Dockerfile

### Application Won't Start
- Check environment variables: `docker run -e ASPNETCORE_ENVIRONMENT=Development`
- View logs: `docker logs <container-id>`
- Ensure port not in use: `docker ps` and check port bindings

### Database Issues
- Ensure volume is mounted: `-v gentralretien-db:/app/data`
- Check SQLite file permissions in volume
- For local dev, run migrations: `dotnet ef database update` before Docker build

## Further Reading

- [Docker Documentation](https://docs.docker.com/)
- [ASP.NET in Docker](https://learn.microsoft.com/en-us/dotnet/core/docker/introduction)
- [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)
