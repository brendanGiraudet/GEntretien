# GEntretien - Gestion d'Équipement de Maintenance

**Un système moderne de gestion d'entretien de matériel construit avec Blazor Server .NET 10 et Google OAuth 2.0.**

---

## 📸 Aperçu

- ✅ **Authentification complète** — Google OAuth 2.0
- ✅ **Interface Blazor Server** — Real-time updates
- ✅ **Base de données SQLite** — Avec migrations EF Core
- ✅ **Validation robuste** — FluentValidation + UI
- ✅ **Architecture Clean** — 4 couches avec SOLID
- ✅ **Docker Ready** — Compose + Multi-stage build
- ✅ **Bien documentée** — Guides pour tous les niveaux

---

## 🚀 Démarrer Rapidement

### 1️⃣ Prérequis
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Google Account (pour OAuth)

### 2️⃣ Obtenir Google OAuth Credentials
Visitez [docs/GOOGLE_OAUTH.md](docs/GOOGLE_OAUTH.md) pour instructions détaillées.

### 3️⃣ Configurer les Secrets

```powershell
# Automatique
.\scripts\SETUP_SECRETS.ps1

# Ou manuel
cd GEntretien
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_SECRET"
```

### 4️⃣ Lancer

```bash
dotnet run --project GEntretien/
# Accédez à: http://localhost:5000
```

---

## 📚 Documentation

### Démarrage
- **[docs/GETTING_STARTED.md](docs/GETTING_STARTED.md)** — Démarrage 5 minutes ⭐

### Guides Techniques
- **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)** — Architecture Clean & SOLID
- **[docs/GOOGLE_OAUTH.md](docs/GOOGLE_OAUTH.md)** — Configuration Google OAuth
- **[docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)** — Problèmes et solutions

### Déploiement
- **[deployment/DOCKER.md](deployment/DOCKER.md)** — Docker local et production
- **[deployment/SECRETS_MANAGEMENT.md](deployment/SECRETS_MANAGEMENT.md)** — Sécurité des secrets

### Ancien docs (consultez plutôt la structure ci-dessus)
- `README.md` (ancien mais conservé pour référence)
- Fichiers `.md` dans la racine (migrer vers `docs/` ou `deployment/`)

---

## 🐳 Docker

### Développement Local

```bash
cp .env.example .env
# Éditer .env avec vos credentials

docker-compose up -d
```

### Production

Voir [deployment/DOCKER.md](deployment/DOCKER.md) et [deployment/SECRETS_MANAGEMENT.md](deployment/SECRETS_MANAGEMENT.md).

---

## 🏗️ Structure du Projet

```
GEntretien/
├── Domain/                 # Entités métier (sans dépendances)
├── Application/            # Validators, Services, DTOs
├── Infrastructure/         # EF Core, Repositories
├── Components/             # Blazor UI
├── Program.cs              # DI + Middleware (OAuth)
└── appsettings.json

docs/                       # 📚 Documentation
├── GETTING_STARTED.md      # Démarrage rapide
├── ARCHITECTURE.md         # Structure et principes
├── GOOGLE_OAUTH.md         # Configuration OAuth
└── TROUBLESHOOTING.md      # Problèmes + solutions

deployment/                 # 🚀 Déploiement
├── DOCKER.md               # Docker local & production
└── SECRETS_MANAGEMENT.md   # Sécurité des secrets

scripts/                    # 🔧 Automation
├── SETUP_SECRETS.ps1       # Configure Google OAuth
└── DOCKER_DEPLOY.ps1       # Gère Docker Compose
```

---

## 🔑 Principes

- **SOLID** — Single Responsibility, Open/Closed, Liskov, Interface Segregation, Dependency Inversion
- **KISS** — Keep It Simple, Stupid
- **YAGNI** — You Aren't Gonna Need It
- **Boy Scout** — Leave code cleaner than you found it

---

## 🧪 Tests

```bash
# Tous les tests
dotnet test

# Spécifique
dotnet test GEntretien.Tests/
```

---

## 📋 Commandes Utiles

```bash
# Développement
dotnet run --project GEntretien/
dotnet build
dotnet test

# Docker
docker-compose up -d
docker-compose logs -f
docker-compose down

# Secrets
cd GEntretien
dotnet user-secrets set "key" "value"
dotnet user-secrets list

# Database
dotnet ef migrations add MigrationName -p GEntretien/ -s GEntretien/
dotnet ef database update -p GEntretien/ -s GEntretien/
```

---

## ✨ Features

### Authentification
- ✅ Google OAuth 2.0
- ✅ Session management
- ✅ User profile display
- ✅ Logout functionality

### Data Management
- ✅ Equipment CRUD
- ✅ Form validation
- ✅ Error handling
- ✅ SQLite persistence

### User Experience
- ✅ Real-time updates (Blazor)
- ✅ Responsive design
- ✅ French labels
- ✅ Dropdown menus

---

## 🔐 Sécurité

- ✅ Google OAuth (industry standard)
- ✅ Secrets protected (.gitignore)
- ✅ HTTPS ready
- ✅ Session cookies
- ✅ CSP headers (à configurer)

Pour production: Voir [deployment/SECRETS_MANAGEMENT.md](deployment/SECRETS_MANAGEMENT.md).

---

## 🚀 Déploiement

### Local
```bash
dotnet run --project GEntretien/
```

### Docker
```bash
docker-compose up -d
```

### Production
Voir [deployment/DOCKER.md](deployment/DOCKER.md) (Swarm) et [deployment/SECRETS_MANAGEMENT.md](deployment/SECRETS_MANAGEMENT.md) (Secrets).

---

## 📞 Support

| Question | Réponse |
|----------|--------|
| Comment démarrer? | [docs/GETTING_STARTED.md](docs/GETTING_STARTED.md) |
| Où est la structure? | [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) |
| OAuth help | [docs/GOOGLE_OAUTH.md](docs/GOOGLE_OAUTH.md) |
| Problématique? | [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) |
| Docker? | [deployment/DOCKER.md](deployment/DOCKER.md) |
| Secrets? | [deployment/SECRETS_MANAGEMENT.md](deployment/SECRETS_MANAGEMENT.md) |

---

## 🧠 Stack Technologique

| Technologie | Versio |
|---|---|
| .NET | 10.0 |
| Blazor Server | Built-in |
| Entity Framework Core | 8.0.10 |
| SQLite | 3.x |
| FluentValidation | 11.9.1 |
| Google OAuth | OIDC |
| Docker | 27.x |
| xUnit | 2.5.3 |

---

## 📈 Roadmap

**Complété:**
- ✅ Blazor scaffolding
- ✅ EF Core + SQLite
- ✅ CRUD Equipment
- ✅ FluentValidation
- ✅ Unit tests
- ✅ Docker setup
- ✅ Google OAuth

**À venir:**
- [ ] MaintenanceRecord entity
- [ ] Location management
- [ ] Maintenance scheduling
- [ ] User profile page
- [ ] Export/Import data
- [ ] Advanced reporting

---

## 🎓 Apprendre

- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)
- [Blazor Server Docs](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

## 📄 License

[À spécifier]

---

**Prêt à démarrer?** Allez à [docs/GETTING_STARTED.md](docs/GETTING_STARTED.md) 🚀
