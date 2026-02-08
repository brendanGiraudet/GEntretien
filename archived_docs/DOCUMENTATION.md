# 📚 Documentation - GEntretien

Bienvenue dans la documentation complète de **GEntretien** - une application de gestion d'entretien de matériel construite avec Blazor .NET 10 et Google OAuth.

## 🚀 Démarrage Rapide

**Nouveau sur le projet?** Commencez par :
1. [QUICK_START.md](QUICK_START.md) — Configuration rapide et déploiement

**Vous avez déjà .NET 10 installé?**
```bash
cd GEntretien
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
dotnet run
```

---

## 📖 Guides Complets

### Configuration et Authentification
- **[GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md)** — Guide détaillé pour obtenir et configurer les credentials Google OAuth 2.0
- **[SETUP_SECRETS.ps1](SETUP_SECRETS.ps1)** — Script PowerShell automatisé pour configurer les secrets

### Déploiement et Infrastructure
- **[QUICK_START.md](QUICK_START.md)** — Configuration rapide (local, Docker, production)
- **[DOCKER_SECRETS.md](DOCKER_SECRETS.md)** — Gestion avancée des secrets pour Docker Swarm et Kubernetes
- **[DOCKER_DEPLOY.ps1](DOCKER_DEPLOY.ps1)** — Script PowerShell pour gérer les containers Docker

### Architecture et Développement
- **[README.md](README.md)** — Documentation générale du projet, structure et principes SOLID

---

## 🎯 Guides par Cas d'Usage

### Je veux...

#### ✅ Démarrer en développement local
→ [QUICK_START.md](QUICK_START.md#développement-local) ou `dotnet run`

#### ✅ Configurer Google OAuth
→ [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md)

#### ✅ Utiliser Docker Compose localement
→ [QUICK_START.md](QUICK_START.md#docker-local)

#### ✅ Déployer en production
→ [QUICK_START.md](QUICK_START.md#production)

#### ✅ Gérer les secrets de manière sécurisée
→ [DOCKER_SECRETS.md](DOCKER_SECRETS.md#production-avec-secrets)

#### ✅ Déployer sur Kubernetes
→ [DOCKER_SECRETS.md](DOCKER_SECRETS.md#kubernetes)

#### ✅ Dépanner une erreur
→ [QUICK_START.md](QUICK_START.md#troubleshooting)

---

## 🏗️ Architecture du Projet

```
GEntretien/
├── Domain/                          # Entités métier (sans dépendances externes)
│   ├── Entities/
│   │   └── Equipment.cs
│   └── Interfaces/
│       └── IEquipmentRepository.cs
├── Application/                     # Use cases, validators, DTOs
│   └── Validators/
│       └── EquipmentValidator.cs
├── Infrastructure/                  # EF Core, repositories, migrations
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/
│   └── Repositories/
│       └── EquipmentRepository.cs
├── Components/                      # Razor composants Blazor
│   ├── App.razor                    # Root avec CascadingAuthenticationState
│   ├── Routes.razor                 # Routing avec AuthorizeRouteView
│   ├── Pages/
│   │   ├── Home.razor
│   │   ├── Login.razor              # ✨ NEW: Google Sign-In page
│   │   └── ...
│   └── Layout/
│       ├── MainLayout.razor
│       └── NavMenu.razor             # ✨ UPDATED: User dropdown + logout
├── Program.cs                       # DI + middleware (avec OAuth)
├── appsettings.json                 # Configuration (secrets via env)
└── Dockerfile                       # Multi-stage build
```

---

## 🔐 Authentification Google OAuth

### Statut: ✅ Complètement configurée

L'application est **100% protégée par Google OAuth**:
- ✅ Tous les routes nécessitent une authentification
- ✅ Login automatiqueswith Google
- ✅ Affichage du profil utilisateur dans le navbar
- ✅ Logout avec suppression de la session

### Flow d'authentification
1. Utilisateur → visite `http://localhost:5000`
2. Middleware → redirige vers `/login` (pas authentifié)
3. Page `Login.razor` → affiche bouton "Se connecter avec Google"
4. Utilisateur → clique et accepte chez Google
5. Google OAuth → retourne au `/signin-google`
6. Program.cs → valide le token et crée une session
7. **Authentifié** → peut accéder à l'application
8. NavMenu → affiche nom, email, bouton déconnexion

### Configuration nécessaire

**Étapes requises** (faire AVANT de tester):
1. Créer un projet Google Cloud (https://console.cloud.google.com)
2. Activer Google+ API
3. Créer des credentials OAuth 2.0 (Web application)
4. Copier Client ID et Client Secret
5. Ajouter aux user secrets:
   ```bash
   dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_ID"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_SECRET"
   ```
6. Lancer: `dotnet run`

> **Besoin d'aide?** Voir [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md)

---

## 📊 Technologie Stack

| Layer | Technologie | Version |
|-------|-------------|---------|
| **Framework** | Blazor Server | .NET 10 |
| **Database** | SQLite | 3.x |
| **ORM** | Entity Framework Core | 8.0.10 |
| **Validation** | FluentValidation | 11.9.1 |
| **Validation UI** | Blazored.FluentValidation | 2.2.0 |
| **Authentication** | Google OAuth 2.0 | OIDC |
| **Authorization** | ASP.NET Core Identity | Built-in |
| **Testing** | xUnit | 2.5.3 |
| **Container** | Docker | 27.x |
| **Orchestration** | Docker Compose / Kubernetes | - |

---

## 📋 Checklist de Démarrage

- [ ] **Installer .NET 10 SDK** → [https://dotnet.microsoft.com](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [ ] **Créer Google OAuth app** → [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md)
- [ ] **Configurer les secrets** → `.\SETUP_SECRETS.ps1` ou manuel
- [ ] **Lancer l'app** → `dotnet run`
- [ ] **Tester le login** → Accédez à http://localhost:5000
- [ ] **Explorer le code** → Voir structure dans [README.md](README.md)

---

## 🔧 Commandes Utiles

### Développement
```bash
# Restaurer les dépendances
dotnet restore

# Builder
dotnet build

# Lancer
dotnet run --project GEntretien/

# Tests
dotnet test
```

### Secrets (User Secrets)
```bash
cd GEntretien

# Initialiser
dotnet user-secrets init

# Ajouter/Modifier
dotnet user-secrets set "key" "value"

# Afficher
dotnet user-secrets list
```

### Docker
```bash
# Docker Compose
docker-compose up -d
docker-compose logs -f
docker-compose down

# Ou avec le script PowerShell
.\DOCKER_DEPLOY.ps1 up
.\DOCKER_DEPLOY.ps1 logs
.\DOCKER_DEPLOY.ps1 down
```

### Database (Entity Framework)
```bash
# Créer une migration
dotnet ef migrations add MigrationName \
  -p GEntretien/GEntretien.csproj \
  -s GEntretien/GEntretien.csproj

# Appliquer les migrations
dotnet ef database update \
  -p GEntretien/GEntretien.csproj \
  -s GEntretien/GEntretien.csproj
```

---

## 🤝 Contributing

Contribution guidelines:
1. **Branch**: Crear une branche pour votre feature (`feature/my-feature`)
2. **Code**: Respecter la structure Clean Architecture
3. **Tests**: Ajouter des tests pour les nouvelles fonctionnalités
4. **Commit**: Messages clairs et descriptifs
5. **Pull Request**: Décrire vos changements et l'impact

Principes de développement:
- **SOLID** — Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **KISS** — Keep It Simple, Stupid
- **YAGNI** — You Aren't Gonna Need It
- **Boy Scout** — Leave code cleaner than you found it

---

## 🆘 Troubleshooting

### Erreur de Login
1. Vérifier les credentials Google dans user-secrets: `dotnet user-secrets list`
2. Vérifier que `http://localhost:5000/signin-google` est dans Authorized Redirect URIs
3. Vérifier les logs: `dotnet run` and look for "Authentication" errors

### Port en utilisation
```powershell
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

### Database locked
- Fermer les instances de l'application (`Ctrl+C`)
- Supprimer `app.db` (elle sera recréée)
- Relancer: `dotnet run`

### Docker issues
- Supprimer les containers : `docker-compose down`
- Rebuild : `docker-compose up -d --build`
- Vérifier les logs : `docker-compose logs gentralretien`

> **Besoin d'aide?** Consulter [QUICK_START.md#troubleshooting](QUICK_START.md#troubleshooting)

---

## 📞 Support

- **Documentation** → Lire les `.md` files
- **Code** → Explorez les dossiers Domain, Application, Infrastructure
- **Issues** → Créer une issue avec logs et détails
- **Google OAuth** → [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md)

---

## 📜 License

[À ajouter]

---

## 🎓 Ressources

- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)
- [Blazor Server Guide](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Google OAuth 2.0](https://developers.google.com/identity/protocols/oauth2)
- [Docker](https://docs.docker.com/)
- [Kubernetes](https://kubernetes.io/docs/)

---

**Dernière mise à jour**: 2025  
**Version**: 1.0.0 avec Google OAuth  
**Statut**: ✅ Production-ready
