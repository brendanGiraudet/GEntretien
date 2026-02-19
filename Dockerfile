# Multi-stage Dockerfile for GEntretien Blazor Server .NET 10 Application
# Optimized for Blazor Server with SignalR, OAuth, and SQLite
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

# Build and publish the application with Blazor optimizations
WORKDIR /src/GEntretien
RUN dotnet publish -c Release -o /app/publish --no-restore \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false \
    /p:PublishReadyToRun=false \
    /p:BlazorEnableCompression=true \
    /p:BlazorWebAssemblyPreserveCollationData=false


# ============================================================================
# Stage 3: Runtime
# ============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Metadata labels
LABEL maintainer="GEntretien Team"
LABEL description="GEntretien - Equipment Maintenance Management System (Blazor Server)"
LABEL version="1.3.14.0"

# Set working directory
WORKDIR /app

# Install runtime dependencies (SQLite, curl for health check)
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       sqlite3 \
       curl \
       ca-certificates \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean

# Create application user (non-root for security in production)
# Disabled for development due to Docker volume permission issues
# Uncomment for production:
# RUN groupadd -r appuser && useradd -r -g appuser -u 1000 appuser

# Create directories with proper permissions
RUN mkdir -p /app/data/keys /app/wwwroot

# Copy published application from build stage
COPY --from=build /app/publish .

# Set permissions for data directory (needed for SQLite and Data Protection)
RUN chmod -R 777 /app/data

# Run as root (development mode - use dedicated user in production)
# For production, uncomment the following line:
# USER appuser

# Expose ports (HTTP and HTTPS)
EXPOSE 5000 5001

# Environment variables optimized for Blazor Server
ENV ASPNETCORE_URLS=http://+:5000 \
    ASPNETCORE_ENVIRONMENT=Development \
    DOTNET_RUNNING_IN_CONTAINER=true \
    # SignalR/Blazor Server optimizations
    Logging__LogLevel__Microsoft.AspNetCore.SignalR=Information \
    Logging__LogLevel__Microsoft.AspNetCore.Http.Connections=Information \
    # Keep-alive for long-running Blazor connections
    CircuitOptions__DisconnectedCircuitRetentionPeriod=00:03:00 \
    CircuitOptions__DisconnectedCircuitMaxRetained=100 \
    # Increase timeouts for SignalR connections
    HttpContext__WebSocketOptions__KeepAliveInterval=00:00:15

# Health check for Blazor Server application
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:5000/ > /dev/null 2>&1 || exit 1

# Run the application
ENTRYPOINT ["dotnet", "GEntretien.dll"]

