# 🎯 Prochaines Étapes - Démarrer GEntretien

Félicitations! Google OAuth est maintenant **complètement configuré**. Voici comment démarrer:

---

## ⚡ Démarrage Rapide (5 minutes)

### 1️⃣ Obtenir les Credentials Google

**Visitez**: https://console.cloud.google.com/

1. Créer un nouveau project
2. Activer **Google+ API**
3. Créer **Credentials** (OAuth 2.0 Client ID - Web application)
4. Ajouter Authorized Redirect URIs:
   ```
   http://localhost:5000/signin-google
   http://localhost:5001/signin-google
   ```
5. Copier le **Client ID** et **Client Secret**

> **Besoin d'aide?** Voir [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md)

### 2️⃣ Configurer les Secrets (Choisir une option)

**Option A: Automatique (powershell)**
```powershell
.\SETUP_SECRETS.ps1
```

**Option B: Manuel**
```bash
cd GEntretien

dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
```

### 3️⃣ Lancer l'Application

```bash
dotnet run --project GEntretien/
```

**Accédez à**: http://localhost:5000

✨ Vous serez automatiquement redirigé vers `/login`  
✨ Cliquez sur "Se connecter avec Google"  
✨ Acceptez et revenez à l'application, authentifié!

---

## 🐳 Avec Docker (Alternatif)

```bash
# 1. Créer le fichier .env
cp .env.example .env

# 2. Éditer .env et ajouter vos credentials
# GOOGLE_CLIENT_ID=YOUR_ID
# GOOGLE_CLIENT_SECRET=YOUR_SECRET

# 3. Lancer
docker-compose up -d

# 4. Vérifier les logs
docker-compose logs -f gentralretien
```

**Accédez à**: http://localhost:5000

---

## 📚 Documentation Disponible

| Guide | Description | Durée |
|-------|------------|-------|
| [QUICK_START.md](QUICK_START.md) | Configuration rapide | 5 min |
| [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md) | Détails Google OAuth | 10 min |
| [DOCUMENTATION.md](DOCUMENTATION.md) | Index complet | - |
| [README.md](README.md) | Architecture du projet | 15 min |
| [DOCKER_SECRETS.md](DOCKER_SECRETS.md) | Secrets avancé (prod) | 20 min |

---

## 🔍 Vérifier que Tout Fonctionne

### Terminal 1: Lancer l'app
```bash
dotnet run --project GEntretien/
```

### Terminal 2: Tester les endpoints
```bash
# Test login endpoint
curl http://localhost:5000/login

# Test app (unauthorized)
curl http://localhost:5000/equipment

# Test app (après login via browser)
# Accédez à http://localhost:5000 dans votre browser
```

### Browser
1. Aller à http://localhost:5000
2. Vous serez redirigé à /login
3. Cliquer "Se connecter avec Google"
4. Accepter les permissions
5. ✅ Vous êtes authentifiés!

---

## 🎨 Aperçu de l'Application

Une fois authentifié, vous avez accès à:

- **Home** — Page d'accueil
- **Counter** — Compteur interactif (demo Blazor)
- **Weather** — Prévisions météo (demo Blazor)
- **Equipment** — Gestion d'équipement (votre feature)
  - Voir la liste
  - Créer un nouvel équipement
  - Modifier un équipement
  - Supprimer un équipement

Chaque action utilise FluentValidation pour valider les données.

---

## 🚀 Prochaines Étapes (Après)

### Pendant le développement:
1. Explorer le code (voir [README.md](README.md))
2. Ajouter de nouveaux domaines (MaintenanceRecord, Location, etc.)
3. Implémenter des validateurs
4. Ajouter des tests

### Avant le déploiement:
1. Configurer HTTPS
2. Mettre à jour Google OAuth Redirect URIs pour votre domaine
3. Configurer les secrets en production (Vault, Docker Secrets, etc.)
4. Tester sur un environnement de staging

### Pour déployer:
- [DOCKER_SECRETS.md](DOCKER_SECRETS.md) — Déployer sur Docker Swarm ou Kubernetes
- [QUICK_START.md#production](QUICK_START.md#production) — Guide production rapide

---

## 🐛 SI QUELQUE CHOSE NE FONCTIONNE PAS

### Erreur "Invalid Client ID"
```bash
# Vérifier les secrets
cd GEntretien
dotnet user-secrets list

# Vérifier que vos credentials sont corrects
# https://console.cloud.google.com/
```

### Erreur "Signature validation failed"
- Vérifier que Client Secret est exact (copier/coller sans espaces)
- Vérifier que vous n'avez pas typographié

### Port 5000 déjà utilisé
```powershell
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Consulter la doc de troubleshooting
→ [QUICK_START.md#troubleshooting](QUICK_START.md#troubleshooting)

---

## 📋 Checklist avant Lancement

- [ ] Google OAuth app créée sur Google Cloud
- [ ] Client ID copié
- [ ] Client Secret copié
- [ ] Secrets configurés (user-secrets)
- [ ] Application peut être lancée (`dotnet run`)
- [ ] http://localhost:5000 fonctionne
- [ ] Login page visible
- [ ] Vous pouvez vous connecter avec Google
- [ ] Navbar affiche votre nom et email
- [ ] Vous pouvez vous déconnecter

---

## 💡 Conseils

### Développement
- Utiliser `dotnet run` pour itération rapide
- Lire le code dans [README.md](README.md) pour comprendre l'architecture
- Modifier les Razor pages dans `GEntretien/Components/`

### Secrets
- Ne JAMAIS commiter les secrets dans git
- `.env` et `appsettings.*.json` sont dans `.gitignore`
- Pour la production, utiliser Docker Secrets ou Vault

### Docker
- Pour développement: `docker-compose up -d`
- Pour production: voir [DOCKER_SECRETS.md](DOCKER_SECRETS.md)

---

## ❓ Besoin d'Aide?

1. **Lire la doc** — [DOCUMENTATION.md](DOCUMENTATION.md) (index complet)
2. **Troubleshooting** — [QUICK_START.md#troubleshooting](QUICK_START.md#troubleshooting)
3. **Google OAuth** — [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md)
4. **Architecture** — [README.md](README.md)

---

## 🎉 Vous êtes Prêts!

Tout est configuré. Il ne manque que les credentials Google.

**Prochaine étape**: Créer l'app Google OAuth et lancer `dotnet run` 🚀

---

**Questions?** Consultez la [DOCUMENTATION.md](DOCUMENTATION.md) pour tous les guides disponibles.
