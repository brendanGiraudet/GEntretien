# 🚀 Démarrage Rapide

**Vous êtes nouveau? Commencez ici!**

---

## ⚡ 5 Minutes pour Démarrer

### 1️⃣ Prérequis
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Google Account (pour OAuth)

### 2️⃣ Obtenir Google OAuth Credentials

Visitez: https://console.cloud.google.com/
- Créer un nouveau project
- Activer Google+ API
- Créer OAuth 2.0 Web credentials
- Ajouter Authorized Redirect URIs:
  ```
  http://localhost:5000/signin-google
  http://localhost:5001/signin-google
  ```
- Copier **Client ID** et **Client Secret**

Voir [GOOGLE_OAUTH.md](GOOGLE_OAUTH.md) pour guide détaillé.

### 3️⃣ Configurer les Secrets

**Option A: Automatique (PowerShell)**
```powershell
cd RepoTest
.\scripts\SETUP_SECRETS.ps1
```

**Option B: Manuel**
```bash
cd GEntretien
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
```

### 4️⃣ Lancer l'Application

```bash
cd GEntretien
dotnet run
```

Accédez à: **http://localhost:5000**

Vous serez automatiquement redirigé vers `/login` où vous pourrez cliquer "Se connecter avec Google".

---

## 🐳 Avec Docker

```bash
# 1. Créer le fichier .env
cp .env.example .env

# 2. Éditer .env et ajouter vos credentials Google
# GOOGLE_CLIENT_ID=YOUR_ID
# GOOGLE_CLIENT_SECRET=YOUR_SECRET

# 3. Lancer
docker-compose up -d

# 4. Accédez à
# http://localhost:5000
```

Voir [../deployment/DOCKER.md](../deployment/DOCKER.md) pour plus d'options.

---

## 📚 Prochaines Étapes

| Voulez-vous... | Allez à... |
|---|---|
| Comprendre l'architecture | [ARCHITECTURE.md](ARCHITECTURE.md) |
| Écrire du code | [../README.md](../README.md) |
| Dépanner une erreur | [TROUBLESHOOTING.md](TROUBLESHOOTING.md) |
| Utiliser Docker | [../deployment/DOCKER.md](../deployment/DOCKER.md) |
| Déployer en production | [../deployment/SECRETS_MANAGEMENT.md](../deployment/SECRETS_MANAGEMENT.md) |
| Configurer Google OAuth | [GOOGLE_OAUTH.md](GOOGLE_OAUTH.md) |

---

## 🔒 Points de Sécurité

- ✅ Ne **jamais** commiter `.env` ou `appsettings.*.json`
- ✅ Utiliser `dotnet user-secrets` pour développement local
- ✅ Utiliser environment variables ou secrets manager en production

Voir [../deployment/SECRETS_MANAGEMENT.md](../deployment/SECRETS_MANAGEMENT.md) pour production.

---

## 🆘 Erreurs Courantes

| Erreur | Solution |
|--------|----------|
| "Invalid Client ID" | Vérifier: `dotnet user-secrets list` |
| Port 5000 en utilisation | Voir [TROUBLESHOOTING.md](TROUBLESHOOTING.md) |
| Database locked | Supprimer `app.db` et relancer |

Voir [TROUBLESHOOTING.md](TROUBLESHOOTING.md) pour plus.

---

## 📖 Documentation Index

- **[ARCHITECTURE.md](ARCHITECTURE.md)** — Structure du projet et principes SOLID
- **[GOOGLE_OAUTH.md](GOOGLE_OAUTH.md)** — Configuration détaillée Google OAuth
- **[TESTING.md](TESTING.md)** — Guide des tests
- **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** — Problèmes et solutions
- **[../deployment/DOCKER.md](../deployment/DOCKER.md)** — Docker local et production
- **[../deployment/SECRETS_MANAGEMENT.md](../deployment/SECRETS_MANAGEMENT.md)** — Secrets en production

---

**Prêt?** Lancez `dotnet run` dans le dossier `GEntretien/` et accédez à http://localhost:5000 🎉
