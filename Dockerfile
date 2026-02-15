# Multi-stage Dockerfile for GEntretien Blazor .NET 10 Application
# Following Docker best practices: small image size, security, caching efficiency

# ============================================================================
# Stage 1: Restore Dependencies
# ============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

# Copy project files only (for better caching)
COPY ["GEntretien.slnx", "GEntretien.slnx"]
COPY ["GEntretien/GEntretien.csproj", "GEntretien/"]
COPY ["GEntretien.Tests/GEntretien.Tests.csproj", "GEntretien.Tests/"]

# Restore NuGet packages (cached if only code changes)
RUN dotnet restore "GEntretien/GEntretien.csproj" --verbosity quiet


# ============================================================================
# Stage 2: Build and Publish
# ============================================================================
FROM restore AS build
WORKDIR /src

# Copy source code
COPY ["GEntretien/", "GEntretien/"]
COPY ["GEntretien.Tests/", "GEntretien.Tests/"]

# Build and publish the application
WORKDIR /src/GEntretien
RUN dotnet publish -c Release -o /app/publish --no-restore \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false \
    /p:PublishReadyToRun=false


# ============================================================================
# Stage 3: Runtime
# ============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Metadata labels
LABEL maintainer="GEntretien Team"
LABEL description="GEntretien - Equipment Maintenance Management System"
LABEL version="1.0"

# Set working directory
WORKDIR /app

# Install runtime dependencies (SQLite, curl for health check)
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       sqlite3 \
       curl \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean

# Create application user (non-root for security in production)
# Disabled for development due to Docker volume permission issues
# RUN groupadd -r appuser && useradd -r -g appuser appuser

# Create directories with proper permissions
RUN mkdir -p /app/data/keys

# Copy published application from build stage
COPY --from=build /app/publish .

# Set permissions for data directory (needed for SQLite)
RUN chmod -R 777 /app/data

# Run as root (development only - use dedicated user in production)
# USER appuser

# Expose ports (HTTP and HTTPS)
EXPOSE 5000 5001

# Environment variables
ENV ASPNETCORE_URLS=http://+:5000 \
    ASPNETCORE_ENVIRONMENT=Development \
    DOTNET_RUNNING_IN_CONTAINER=true

# Health check using curl
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:5000/ > /dev/null 2>&1 || exit 1

# Run the application
ENTRYPOINT ["dotnet", "GEntretien.dll"]

