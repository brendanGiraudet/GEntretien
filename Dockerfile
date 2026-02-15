# Multi-stage Dockerfile for GEntretien Blazor .NET 10 Application

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["GEntretien.slnx", "GEntretien.slnx"]
COPY ["GEntretien/GEntretien.csproj", "GEntretien/"]
COPY ["GEntretien.Tests/GEntretien.Tests.csproj", "GEntretien.Tests/"]

# Restore dependencies
RUN dotnet restore "GEntretien/GEntretien.csproj"

# Copy source code
COPY ["GEntretien/", "GEntretien/"]
COPY ["GEntretien.Tests/", "GEntretien.Tests/"]

# Build and publish the application
WORKDIR /src/GEntretien
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install SQLite support (optional but recommended for production)
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

# Copy published application from build stage
COPY --from=build /app/publish .

# Create directories for SQLite database and Data Protection keys
RUN mkdir -p /app/data/keys

# Expose port (Blazor Server typically uses 5000/HTTP and 5001/HTTPS)
EXPOSE 5000 5001

# Environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD dotnet --version || exit 1

# Run the application
ENTRYPOINT ["dotnet", "GEntretien.dll"]
