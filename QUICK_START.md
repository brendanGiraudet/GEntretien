# 🚀 Configuration et Déploiement Rapide - GEntretien

Ce guide vous aide à configurer et déployer GEntretien rapidement.

## 📋 Sommaire

1. [Développement Local](#développement-local)
2. [Docker Local](#docker-local)
3. [Production](#production)
4. [Troubleshooting](#troubleshooting)

---

## Développement Local

### Prérequis

- **[.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)**
- **Google OAuth Credentials** (voir [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md))

### Installation

```bash
# 1. Cloner le repo
git clone <repo-url>
cd RepoTest

# 2. Restaurer les dépendances
dotnet restore

# 3. Configurer les secrets Google OAuth (voir section ci-dessous)
cd GEntretien
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"

# 4. Lancer l'application
dotnet run
```

**Accéder à:**
- Application: http://localhost:5000
- Login: http://localhost:5000/login

### Obtenir les Credentials Google

> **RAPIDE**: Pour le développement local avec user secrets:

```powershell
# Windows PowerShell
.\SETUP_SECRETS.ps1
```

```bash
# Linux/Mac
chmod +x SETUP_SECRETS.ps1
./SETUP_SECRETS.ps1
```

> **MANUEL**: Voir [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md) pour les étapes détaillées

---

## Docker Local

### Setup Rapide

```bash
# 1. Créer le fichier .env
cp .env.example .env

# 2. Éditer .env avec vos Google OAuth credentials
# Éditer .env et remplacer:
#   GOOGLE_CLIENT_ID=YOUR_CLIENT_ID_HERE
#   GOOGLE_CLIENT_SECRET=YOUR_CLIENT_SECRET_HERE

# 3. Lancer avec Docker Compose
docker-compose up -d

# 4. Vérifier les logs
docker-compose logs -f gentralretien
```

**Accéder à:** http://localhost:5000

### Avec le script PowerShell

```powershell
# Démarrer
.\DOCKER_DEPLOY.ps1 up

# Voir les logs
.\DOCKER_DEPLOY.ps1 logs

# Arrêter
.\DOCKER_DEPLOY.ps1 down

# Redémarrer
.\DOCKER_DEPLOY.ps1 restart
```

---

## Production

### Checklist

- [ ] Google OAuth credentials configurés
- [ ] `.env` créé et rempli (ne JAMAIS commiter)
- [ ] `.gitignore` contient `.env` et `appsettings.*.json`
- [ ] HTTPS/SSL configuré
- [ ] Authorized Redirect URIs incluent votre domaine
- [ ] Docker image buildée et testée

### Avec Docker Swarm (Simple)

```bash
# 1. Créer les secrets
echo "YOUR_CLIENT_ID" | docker secret create google_client_id -
echo "YOUR_CLIENT_SECRET" | docker secret create google_client_secret -

# 2. Initialiser Swarm (si pas déjà fait)
docker swarm init

# 3. Déployer
docker stack deploy -c docker-compose.prod.yml gentralretien

# 4. Vérifier
docker service ls
docker service logs gentralretien_gentralretien
```

### Avec Kubernetes (Avancé)

```bash
# 1. Créer le secret Kubernetes
kubectl create secret generic gentralretien-secrets \
  --from-literal=google_client_id=YOUR_CLIENT_ID \
  --from-literal=google_client_secret=YOUR_CLIENT_SECRET \
  -n production

# 2. Déployer
kubectl apply -f k8s-deployment.yaml

# 3. Vérifier
kubectl get pods -n production
kubectl logs -f deployment/gentralretien -n production
```

> **Plus d'infos:** Voir [DOCKER_SECRETS.md](DOCKER_SECRETS.md)

---

## Troubleshooting

### Application n'est pas accessible

```bash
# Vérifier que le container est en cours d'exécution
docker ps | grep gentralretien

# Vérifier les logs
docker logs gentralretien

# Tester la connexion
curl http://localhost:5000
```

### Google Sign-In ne fonctionne pas

1. **Vérifier les credentials**
   ```bash
   # Développement avec user secrets
   cd GEntretien
   dotnet user-secrets list
   
   # Docker Compose
   docker exec gentralretien env | grep -i google
   ```

2. **Vérifier les Authorized Redirect URIs**
   - Google Cloud Console → Credentials → OAuth 2.0 Client IDs
   - Vérifier que `http://localhost:5000/signin-google` est présent
   - En production: `https://yourdomain.com/signin-google`

3. **Vérifier les logs**
   ```bash
   dotnet run --project GEntretien/
   # Chercher "Authentication" ou "Google" dans les logs
   ```

### Port déjà en utilisation

```powershell
# Windows: Trouver quelle application utilise le port 5000
netstat -ano | findstr :5000

# Arrêter l'application qui utilise le port
taskkill /PID <PID> /F

# Ou utiliser un port différent
docker run -p 5001:5000 gentralretien
```

### Database locked

```bash
# SQLite ne supporte pas bien les accès concurrents dans Docker
# Solution 1: Utiliser un volume bind
docker run -v ./data:/app/data gentralretien

# Solution 2: Migrer vers PostgreSQL ou SQL Server
# Voir la section "Migrer vers PostgreSQL" dans la documentation
```

### Secrets non lus correctement

```bash
# Vérifier les variables d'environnement dans le container
docker exec gentralretien env | grep -i authentication

# Vérifier que les secrets sont montés (Docker Swarm)
docker exec gentralretien ls -la /run/secrets/

# Vérifier les logs pour les erreurs d'authentification
docker logs gentralretien | grep -i "error\|failed\|invalid"
```

---

## 📚 Documentation Complète

| Document | Description |
|----------|-------------|
| [README.md](README.md) | Aperçu général et architecture du projet |
| [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md) | Guide détaillé pour obtenir les credentials Google |
| [DOCKER_SECRETS.md](DOCKER_SECRETS.md) | Gestion avancée des secrets en production |
| [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md) | Guide complet de déploiement Docker |

---

## ⚡ Commandes Rapides

```bash
# Développement
dotnet run --project GEntretien/

# Tests
dotnet test

# Docker Compose
docker-compose up -d
docker-compose down
docker-compose logs -f

# Docker Swarm
docker stack deploy -c docker-compose.prod.yml gentralretien
docker stack services gentralretien

# Kubernetes
kubectl apply -f k8s-deployment.yaml
kubectl get pods
kubectl logs -f pod/<pod-name>
```

---

## 🔐 Sécurité

### Secrets Management

| Environnement | Méthode | Sécurisé? |
|---|---|---|
| Développement | `dotnet user-secrets` | ✅ Oui |
| Développement | `.env.local` | ⚠️ À gitignore |
| Docker Local | Variables d'environnement | ⚠️ À protéger |
| Docker Swarm | Docker Secrets | ✅ Oui |
| Kubernetes | Kubernetes Secrets | ✅ Oui |
| Production | Vault/AWS Secrets Manager | ✅ Recommandé |

### À NE PAS FAIRE

- ❌ Commiter `.env` ou `appsettings.*.json` en git
- ❌ Hardcoder les secrets dans le code
- ❌ Passer les secrets en ligne de commande (visible dans `ps`)
- ❌ Envoyer les secrets par email ou Slack
- ❌ Utiliser des placeholders comme `TODO` en production

---

## 🆘 Besoin d'aide?

1. **Lire les docs:**
   - [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md) — Configuration Google
   - [DOCKER_SECRETS.md](DOCKER_SECRETS.md) — Gestion des secrets
   - [README.md](README.md) — Architecture et structure

2. **Lancer les scripts:**
   ```powershell
   .\SETUP_SECRETS.ps1      # Pour configurer les secrets
   .\DOCKER_DEPLOY.ps1 logs # Pour voir les logs
   ```

3. **Vérifier les logs:**
   ```bash
   dotnet run --project GEntretien/
   docker logs gentralretien
   kubectl logs -f deployment/gentralretien
   ```

4. **Créer une issue:** Si toutes les docs et logs n'aident pas, créer une issue avec:
   - Environnement (Windows/Linux, Docker/Kubernetes, etc.)
   - Erreur exacte avec stack trace
   - Les étapes que vous avez suivies
   - Output de `dotnet --version`, `docker --version`, etc.

---

**Bonne chance! 🚀**
