# Script de Configuration des Secrets Google OAuth pour GEntretien
# Ce script configure les user secrets de manière sécurisée pour le développement local

Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Configuration des Secrets Google OAuth pour GEntretien         ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Vérifier que dotnet-user-secrets is available
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ dotnet CLI non trouvé. Installez .NET 10 SDK" -ForegroundColor Red
    exit 1
}

Write-Host "✓ .NET $dotnetVersion détecté" -ForegroundColor Green
Write-Host ""

# Naviger dans le dossier du projet
$projectPath = ".\GEntretien"
if (-not (Test-Path $projectPath)) {
    Write-Host "❌ Dossier '$projectPath' non trouvé" -ForegroundColor Red
    exit 1
}

Push-Location $projectPath

# Vérifier si les user secrets sont déjà initialisés
$userSecretsId = dotnet user-secrets --list 2>&1 | Select-String "UserSecretsId"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ User Secrets déjà initialisés" -ForegroundColor Green
} else {
    Write-Host "Initialisation des User Secrets..." -ForegroundColor Yellow
    dotnet user-secrets init
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Erreur lors de l'initialisation des User Secrets" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    Write-Host "✓ User Secrets initialisés" -ForegroundColor Green
}

Write-Host ""
Write-Host "Vous avez deux options pour fournir vos credentials Google :" -ForegroundColor Cyan
Write-Host ""
Write-Host "Option 1️⃣  : Entrer les credentials maintenant"
Write-Host "Option 2️⃣  : Les ajouter manuellement plus tard"
Write-Host ""

$choice = Read-Host "Choisissez 1 ou 2 (défaut: 1)"
if ([string]::IsNullOrWhiteSpace($choice)) {
    $choice = "1"
}

if ($choice -eq "1") {
    Write-Host ""
    Write-Host "Obtenir vos credentials Google :" -ForegroundColor Yellow
    Write-Host "1. Allez sur: https://console.cloud.google.com/" -ForegroundColor Gray
    Write-Host "2. Activez Google+ API"
    Write-Host "3. Créez une OAuth application (Web)"
    Write-Host "4. Copiez le Client ID et Client Secret"
    Write-Host ""
    
    # Vérifier si le fichier GOOGLE_OAUTH_SETUP.md existe pour plus de détails
    if (Test-Path "..\GOOGLE_OAUTH_SETUP.md") {
        Write-Host "📖 Voir GOOGLE_OAUTH_SETUP.md pour un guide détaillé" -ForegroundColor Blue
    }
    
    Write-Host ""
    $clientId = Read-Host "Entrez votre Google Client ID"
    if ([string]::IsNullOrWhiteSpace($clientId)) {
        Write-Host "❌ Client ID vide. Opération annulée" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    
    $clientSecret = Read-Host "Entrez votre Google Client Secret" -AsSecureString
    $clientSecretPlain = [System.Net.NetworkCredential]::new("", $clientSecret).Password
    
    if ([string]::IsNullOrWhiteSpace($clientSecretPlain)) {
        Write-Host "❌ Client Secret vide. Opération annulée" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    
    Write-Host ""
    Write-Host "Configuration des secrets..." -ForegroundColor Yellow
    
    dotnet user-secrets set "Authentication:Google:ClientId" $clientId
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Erreur lors de la configuration du Client ID" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    
    dotnet user-secrets set "Authentication:Google:ClientSecret" $clientSecretPlain
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Erreur lors de la configuration du Client Secret" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    
    Write-Host "✓ Secrets configurés avec succès" -ForegroundColor Green
    
} else {
    Write-Host "Configuration manuelle requise :" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Après obtenir vos credentials Google, exécutez :" -ForegroundColor Gray
    Write-Host "cd .\GEntretien" -ForegroundColor Cyan
    Write-Host "dotnet user-secrets set ""Authentication:Google:ClientId"" ""YOUR_CLIENT_ID""" -ForegroundColor Cyan
    Write-Host "dotnet user-secrets set ""Authentication:Google:ClientSecret"" ""YOUR_CLIENT_SECRET""" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Affichage des secrets actuels :" -ForegroundColor Yellow
Write-Host ""
dotnet user-secrets list

Write-Host ""
Write-Host "⚠️  Attention:" -ForegroundColor Red
Write-Host "  - Les secrets sont stockés dans:" -ForegroundColor Gray
$userSecretsDir = "$env:APPDATA\Microsoft\UserSecrets"
Write-Host "    $userSecretsDir" -ForegroundColor Cyan
Write-Host "  - Ils ne sont visibles QUE sur cette machine en tant que cet utilisateur" -ForegroundColor Gray
Write-Host "  - Pour produire, utilisez des variables d'environnement ou Docker Secrets" -ForegroundColor Gray
Write-Host ""

Write-Host "🚀 Prêt à lancer l'application :" -ForegroundColor Green
Write-Host "   dotnet run" -ForegroundColor Cyan
Write-Host "   Puis accédez à: http://localhost:5000" -ForegroundColor Cyan

Pop-Location
