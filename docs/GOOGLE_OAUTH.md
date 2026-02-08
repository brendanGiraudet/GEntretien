# 🔐 Configuration Google OAuth 2.0

Guide complet pour obtenir et configurer les credentials Google OAuth.

---

## 📋 Étapes

### 1️⃣ Google Cloud Console

1. Visitez: https://console.cloud.google.com/
2. Connectez-vous avec votre compte Google
3. Créez un nouveau **Project**:
   - Cliquez "Select a Project" → "NEW PROJECT"
   - Nom: "GEntretien"
   - Créez

### 2️⃣ Activez Google+ API

1. Allez à **APIs & Services** → **Library**
2. Cherchez "**Google+ API**"
3. Cliquez dessus
4. Cliquez **ENABLE**

### 3️⃣ OAuth Consent Screen

1. **APIs & Services** → **OAuth consent screen**
2. **User Type:** External
3. Remplissez:
   - **App name:** GEntretien
   - **User support email:** votre-email@example.com
   - **Developer contact:** votre-email@example.com
4. **Scopes:** Ajouter `email` et `profile`
5. Sauvegardez

### 4️⃣ OAuth 2.0 Credentials

1. **APIs & Services** → **Credentials**
2. **+ Create Credentials** → **OAuth client ID**
3. **Application type:** Web application
4. **Name:** GEntretien Web
5. **Authorized JavaScript origins:**
   ```
   http://localhost:5000
   http://localhost:5001
   https://yourdomain.com  (en production)
   ```
6. **Authorized redirect URIs:**
   ```
   http://localhost:5000/signin-google
   http://localhost:5001/signin-google
   https://yourdomain.com/signin-google  (en production)
   ```
7. **Créez**

### 5️⃣ Copier les Credentials

Un dialog affiche:
- **Client ID** → Copy
- **Client Secret** → Copy

**⚠️ Gardez ces valeurs secrètes!** Ne les committez jamais dans git.

---

## 🔧 Configuration locale avec User Secrets

### Option 1: Script Automatique (Recommandé)

```powershell
cd RepoTest
.\scripts\SETUP_SECRETS.ps1
```

Le script vous demandera votre Client ID et Client Secret.

### Option 2: Manuel

```bash
cd GEntretien

# Initialiser (première fois seulement)
dotnet user-secrets init

# Ajouter vos credentials
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"

# Vérifier
dotnet user-secrets list
```

Les secrets sont stockés **localement uniquement**:
- **Windows:** `%APPDATA%\Microsoft\UserSecrets\`
- **Linux/Mac:** `~/.microsoft/usersecrets/`

### Option 3: appsettings.Development.json

⚠️ **Attention:** N'utilisez PAS en production, fichier sensible!

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_CLIENT_ID",
      "ClientSecret": "YOUR_CLIENT_SECRET"
    }
  }
}
```

Assurez-vous que `appsettings.*.json` est dans `.gitignore`.

---

## 🧪 Tester Localement

```bash
# 1. Lancer l'app
cd GEntretien
dotnet run

# 2. Accédez à
# http://localhost:5000

# 3. Vous serez redirigé à /login

# 4. Cliquez "Se connecter avec Google"

# 5. Acceptez les permissions

# 6. ✅ Vous êtes authentifié!
```

---

## 🐳 Avec Docker Compose

```bash
# 1. Créer .env
cp .env.example .env

# 2. Éditer .env
# GOOGLE_CLIENT_ID=YOUR_ID
# GOOGLE_CLIENT_SECRET=YOUR_SECRET

# 3. Lancer
docker-compose up -d

# 4. Accédez à http://localhost:5000
```

Voir [../deployment/DOCKER.md](../deployment/DOCKER.md) pour plus d'options.

---

## ⏳ En Production

Pour la production, **ne JAMAIS** utiliser des variables en clair.

Utilisez:
- **Docker Secrets** (Swarm)
- **Kubernetes Secrets** (K8s)
- **Vault / AWS Secrets Manager** (Enterprise)

Voir [../deployment/SECRETS_MANAGEMENT.md](../deployment/SECRETS_MANAGEMENT.md) pour le guide complet.

---

## 🆘 Troubleshooting

| Problème | Solution |
|----------|----------|
| "Invalid Client ID" | Vérifier que ClientID est exact, pas de typos |
| "Unauthorized client" | Vérifier que Client Secret est exact |
| "Redirect URI mismatch" | Ajouter `http://localhost:5000/signin-google` aux Authorized URIs |
| "Google+ API not enabled" | Vérifier: APIs & Services → Library → Enable Google+ API |

---

## 📚 Ressources

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [Google Cloud Console](https://console.cloud.google.com/)
- [ASP.NET Core Google Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/google-logins)
