# Configuration Google OAuth 2.0 pour GEntretien

## 🔐 Obtenir les Credentials Google

### Étape 1 : Accéder à Google Cloud Console

1. Aller sur [Google Cloud Console](https://console.cloud.google.com/)
2. Se connecter avec un compte Google
3. Créer un nouveau project (ou sélectionner un existant)
4. Nommer le projet (ex: "GEntretien")

### Étape 2 : Activer Google+ API

1. Aller à **APIs & Services** → **Library**
2. Chercher "Google+ API"
3. Cliquer sur "Google+ API"
4. Cliquer sur **Enable**

### Étape 3 : Créer des Credentials OAuth

1. Aller à **APIs & Services** → **Credentials**
2. Cliquer sur **+ Create Credentials** → **OAuth client ID**
3. Si demandé, configurer le **OAuth consent screen** d'abord :
   - **User Type** : External
   - **App name** : GEntretien
   - **User support email** : votre-email@example.com
   - **Developer contact** : votre-email@example.com
   - Ajouter les scopes : `email`, `profile`
4. Une fois le consent screen configuré, créer l'OAuth client ID:
   - **Application type** : Web application
   - **Name** : GEntretien Web
   - **Authorized JavaScript origins** :
     - `http://localhost:5000`
     - `http://localhost:5001`
     - `https://your-domain.com` (en production)
   - **Authorized redirect URIs** :
     - `http://localhost:5000/signin-google`
     - `http://localhost:5001/signin-google`
     - `https://your-domain.com/signin-google` (en production)

### Étape 4 : Copier les Credentials

Un dialog affichera :
- **Client ID** 
- **Client Secret**

Gardez ces valeurs sécurisées (ne JAMAIS les commit dans Git!)

---

## 📋 Configuration locale

### Option 1 : User Secrets (Recommandé pour le développement)

```powershell
# Initialiser user secrets pour le projet
cd GEntretien
dotnet user-secrets init

# Ajouter les secrets (remplacer par vos vraies valeurs)
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID_HERE"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET_HERE"

# Vérifier les secrets
dotnet user-secrets list
```

Les user secrets sont stockés dans `%APPDATA%\Microsoft\UserSecrets\` (Windows) ou `~/.microsoft/usersecrets/` (Linux/Mac).

### Option 2 : appsettings.Development.json

⚠️ **Attention:** Ne JAMAIS commit les secrets en production!

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_CLIENT_ID_HERE",
      "ClientSecret": "YOUR_CLIENT_SECRET_HERE"
    }
  }
}
```

Ajouter `appsettings.Development.json` à `.gitignore`

### Option 3 : Environment Variables

```powershell
$env:Authentication__Google__ClientId="YOUR_CLIENT_ID_HERE"
$env:Authentication__Google__ClientSecret="YOUR_CLIENT_SECRET_HERE"
```

---

## 🚀 Lancer l'application

```powershell
# Développement local
dotnet run --project GEntretien/GEntretien.csproj

# Ou avec Docker Compose
docker-compose up -d
```

Accéder à `http://localhost:5000` et cliquer sur **Se connecter avec Google**.

---

## 🐳 Configuration Docker / Production

### Environment Variables pour Docker

```dockerfile
ENV Authentication__Google__ClientId=YOUR_CLIENT_ID
ENV Authentication__Google__ClientSecret=YOUR_CLIENT_SECRET
```

Ou dans `docker-compose.yml`:

```yaml
environment:
  - Authentication__Google__ClientId=YOUR_CLIENT_ID
  - Authentication__Google__ClientSecret=YOUR_CLIENT_SECRET
```

### ⚠️ Sécurité en Production

1. **Utiliser Docker Secrets** (Swarm/K8s) ou **Variables d'environnement sécurisées**
2. **Ne JAMAIS commiter les secrets dans le repo**
3. **Ajouter HTTPS obligatoire** en production
4. **Configurer CORS et Origin restrictions**

Exemple Docker Secrets (Swarm):

```bash
echo "YOUR_CLIENT_ID" | docker secret create auth_google_client_id -
echo "YOUR_CLIENT_SECRET" | docker secret create auth_google_client_secret -
```

Puis lire dans l'application:

```csharp
var clientId = File.ReadAllText("/run/secrets/auth_google_client_id").Trim();
var clientSecret = File.ReadAllText("/run/secrets/auth_google_client_secret").Trim();
```

---

## GitHub Actions / CI/CD

Pour automatiser avec GitHub Actions, ajouter les secrets au repository:

1. Aller à **Settings** → **Secrets and variables** → **Actions**
2. Cliquer **New repository secret**
3. Ajouter :
   - `GOOGLE_CLIENT_ID`
   - `GOOGLE_CLIENT_SECRET`

Utiliser dans le workflow:

```yaml
env:
  Authentication__Google__ClientId: ${{ secrets.GOOGLE_CLIENT_ID }}
  Authentication__Google__ClientSecret: ${{ secrets.GOOGLE_CLIENT_SECRET }}
```

---

## 🧪 Test local

### Page de Login

Accéder à `http://localhost:5000/login` pour tester la page de connexion.

### Routes Protégées

Accéder à `http://localhost:5000/equipment` (ou toute page) sans s'être connecté :
- Vous serez redirigé vers `/login`

### Après Connexion

Une fois authentifié via Google :
- Votre prénom et email apparaîtront dans le dropdown du navbar
- Vous pouvez cliquer **Déconnexion** pour vous déconnecter

---

## 🐛 Dépannage

### "Invalid Client ID" ou "Unauthorized client"

- Vérifier que ClientID et ClientSecret sont corrects
- Vérifier que `signin-google` est dans les **Authorized redirect URIs**
- Vérifier que **Google+ API** est activée

### "Localhost not allowed"

- La première connexion locale peut nécessiter une approbation
- Ajouter `http://localhost:5000` et `http://localhost:5001` aux **Authorized JavaScript origins**

### Cookies d'authentification persistent

- Les cookies sont stockés côté NavigationManager de Blazor
- Utiliser l'option de "Sign out" du dropdown pour se déconnecter

---

## 📚 Ressources

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [ASP.NET Core Google Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/google-logins)
- [Blazor Server Authentication](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/)
- [User Secrets Manager](https://learn.microsoft.com/en-us/aspnet/core/app-secrets)
