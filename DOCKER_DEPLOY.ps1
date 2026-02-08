# Script PowerShell pour Déployer GEntretien avec Docker Compose
# Utilise les secrets pour passer les variables d'environnement

param(
    [string]$Action = "up",
    [switch]$NoCache,
    [switch]$Verbose
)

function Write-Header {
    param([string]$Text)
    Write-Host ""
    Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║  $Text                                         ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
}

function Test-DockerInstalled {
    $docker = Get-Command docker -ErrorAction SilentlyContinue
    if (-not $docker) {
        Write-Host "❌ Docker n'est pas installé ou pas dans le PATH" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Docker trouvé: $(docker --version)" -ForegroundColor Green
}

function Test-EnvFile {
    if (-not (Test-Path ".env")) {
        Write-Host "⚠️  Fichier .env non trouvé" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Créez un fichier .env avec vos Google OAuth credentials :" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "  1. Copier .env.example en .env :" -ForegroundColor Gray
        Write-Host "     cp .env.example .env" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "  2. Éditer .env et ajouter vos vraies credentials :" -ForegroundColor Gray
        Write-Host "     GOOGLE_CLIENT_ID=YOUR_ID" -ForegroundColor Cyan
        Write-Host "     GOOGLE_CLIENT_SECRET=YOUR_SECRET" -ForegroundColor Cyan
        Write-Host ""
        return $false
    }
    return $true
}

function Get-ComposeCommand {
    $command = "docker-compose"
    
    # Essayer docker compose (v2) d'abord
    $newCompose = docker compose version 2>&1
    if ($LASTEXITCODE -eq 0) {
        $command = "docker compose"
    }
    
    return $command
}

# Main
Write-Header "GEntretien - Docker Deployment Assistant"

Test-DockerInstalled

$ComposeCmd = Get-ComposeCommand
Write-Host "✓ Docker Compose trouvé" -ForegroundColor Green
Write-Host "  Commande: $ComposeCmd" -ForegroundColor Gray

switch ($Action.ToLower()) {
    "up" {
        Write-Header "Démarrage de GEntretien"
        
        if (-not (Test-EnvFile)) {
            exit 1
        }
        
        Write-Host "Vérification de la configuration .env :" -ForegroundColor Yellow
        Get-Content .env | ForEach-Object {
            if ($_ -match "^[^=]+=" -and $_ -notmatch "^#") {
                $key = $_ -split "=|" | Select-Object -First 1
                Write-Host "  ✓ $key" -ForegroundColor Gray
            }
        }
        
        Write-Host ""
        Write-Host "Commande: $ComposeCmd up -d" -ForegroundColor Cyan
        Write-Host ""
        
        if ($NoCache) {
            & cmd /c "$ComposeCmd build --no-cache && $ComposeCmd up -d"
        } else {
            & cmd /c "$ComposeCmd up -d"
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "✅ GEntretien est maintenant en ligne!" -ForegroundColor Green
            Write-Host ""
            Write-Host "Informations:" -ForegroundColor Cyan
            Write-Host "  URL:     http://localhost:5000" -ForegroundColor Gray
            Write-Host "  Logs:    $ComposeCmd logs -f gentralretien" -ForegroundColor Gray
            Write-Host "  Arrêter: $ComposeCmd down" -ForegroundColor Gray
            Write-Host ""
        } else {
            Write-Host "❌ Erreur lors du démarrage" -ForegroundColor Red
            exit 1
        }
    }
    
    "logs" {
        Write-Header "Logs GEntretien"
        & cmd /c "$ComposeCmd logs -f gentralretien"
    }
    
    "down" {
        Write-Header "Arrêt de GEntretien"
        & cmd /c "$ComposeCmd down"
        Write-Host "✓ GEntretien arrêté" -ForegroundColor Green
    }
    
    "restart" {
        Write-Header "Redémarrage de GEntretien"
        & cmd /c "$ComposeCmd restart gentralretien"
        Write-Host "✓ GEntretien redémarré" -ForegroundColor Green
    }
    
    "build" {
        Write-Header "Build de GEntretien"
        $buildCmd = if ($NoCache) { "$ComposeCmd build --no-cache" } else { "$ComposeCmd build" }
        Write-Host "Commande: $buildCmd" -ForegroundColor Cyan
        Write-Host ""
        & cmd /c $buildCmd
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Build terminé avec succès" -ForegroundColor Green
        } else {
            Write-Host "❌ Erreur lors du build" -ForegroundColor Red
            exit 1
        }
    }
    
    "shell" {
        Write-Header "Shell dans le container GEntretien"
        & cmd /c "$ComposeCmd exec gentralretien /bin/bash"
    }
    
    "ps" {
        Write-Header "Status des containers"
        & cmd /c "$ComposeCmd ps"
    }
    
    default {
        Write-Host "Actions disponibles :" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "  .\DOCKER_DEPLOY.ps1              # Démarrer (up)" -ForegroundColor Cyan
        Write-Host "  .\DOCKER_DEPLOY.ps1 up           # Démarrer" -ForegroundColor Cyan
        Write-Host "  .\DOCKER_DEPLOY.ps1 up -NoCache  # Démarrer (rebuild)" -ForegroundColor Cyan
        Write-Host "  .\DOCKER_DEPLOY.ps1 down         # Arrêter" -ForegroundColor Cyan
        Write-Host "  .\DOCKER_DEPLOY.ps1 logs         # Afficher les logs" -ForegroundColor Cyan
        Write-Host "  .\DOCKER_DEPLOY.ps1 restart      # Redémarrer" -ForegroundColor Cyan
        Write-Host "  .\DOCKER_DEPLOY.ps1 build        # Builder l'image" -ForegroundColor Cyan
        Write-Host "  .\DOCKER_DEPLOY.ps1 shell        # Shell dans le container" -ForegroundColor Cyan
        Write-Host "  .\DOCKER_DEPLOY.ps1 ps           # Afficher les containers" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Prérequis :" -ForegroundColor Yellow
        Write-Host "  • Fichier .env avec GOOGLE_CLIENT_ID et GOOGLE_CLIENT_SECRET" -ForegroundColor Gray
        Write-Host "    (copier depuis .env.example et remplir vos credentials)" -ForegroundColor Gray
        Write-Host ""
    }
}
