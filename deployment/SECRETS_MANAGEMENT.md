# 🔐 Gestion Sécurisée des Secrets

Comment gérer vos Google OAuth credentials en production **de manière sécurisée**.

---

## ⚠️ Risques de Sécurité

❌ **JAMAIS faire:**
- Commiter secrets dans git
- Passer secrets en ligne de commande: `docker run -e SECRET=value`
- Stocker secrets en plaintext dans fichiers
- Envoyer secrets par email ou Slack

✅ **À FAIRE:**
- Secrets Manager (Vault, AWS Secrets Manager, etc.)
- Environment variables sécurisées
- Docker Secrets (Swarm)
- Kubernetes Secrets (K8s)

---

## 🔐 Par Environnement

### 1️⃣ Développement Local

**Méthode:** User Secrets (recommandé)

```bash
cd GEntretien

# Initialiser
dotnet user-secrets init

# Ajouter
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_SECRET"

# Vérifier
dotnet user-secrets list
```

**Stockage:** `%APPDATA%\Microsoft\UserSecrets\` (Windows)  
**Avantage:** Machine-specific, jamais commité

---

### 2️⃣ Docker Local

**Méthode:** `.env` file

```bash
cp .env.example .env
# Éditer .env

docker-compose up -d
```

**.env (exemple)**
```env
GOOGLE_CLIENT_ID=YOUR_ID
GOOGLE_CLIENT_SECRET=YOUR_SECRET
ASPNETCORE_ENVIRONMENT=Development
```

**⚠️ Important:** `.env` dans `.gitignore`!

```bash
echo ".env" > .gitignore
echo "appsettings.*.json" >> .gitignore
```

---

### 3️⃣ Docker Swarm (Self-Hosted)

**Méthode:** Docker Secrets

**Créer les secrets:**
```bash
echo "YOUR_CLIENT_ID" | docker secret create google_client_id -
echo "YOUR_CLIENT_SECRET" | docker secret create google_client_secret -
```

**docker-compose.yml (Swarm):**
```yaml
version: '3.8'

services:
  gentralretien:
    image: ghcr.io/yourusername/gentralretien:latest
    environment:
      - Authentication__Google__ClientId_FILE=/run/secrets/google_client_id
      - Authentication__Google__ClientSecret_FILE=/run/secrets/google_client_secret
    secrets:
      - google_client_id
      - google_client_secret

secrets:
  google_client_id:
    external: true
  google_client_secret:
    external: true
```

**Déployer:**
```bash
docker stack deploy -c docker-compose.yml gentralretien
```

**Lire dans l'app:**
```csharp
// Program.cs
string ReadSecret(string path)
{
    try
    {
        return File.ReadAllText(path).Trim();
    }
    catch
    {
        return "";  // Fallback
    }
}

var clientId = ReadSecret("/run/secrets/google_client_id");
var clientSecret = ReadSecret("/run/secrets/google_client_secret");
```

---

### 4️⃣ Kubernetes (Cloud)

**Méthode:** Kubernetes Secrets

**Créer le secret:**
```bash
kubectl create secret generic gentralretien-oauth \
  --from-literal=client-id=YOUR_ID \
  --from-literal=client-secret=YOUR_SECRET \
  -n production
```

**Manifeste Deployment:**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gentralretien
  namespace: production

spec:
  template:
    spec:
      containers:
      - name: gentralretien
        image: ghcr.io/yourusername/gentralretien:1.0.0
        env:
        - name: Authentication__Google__ClientId
          valueFrom:
            secretKeyRef:
              name: gentralretien-oauth
              key: client-id
        - name: Authentication__Google__ClientSecret
          valueFrom:
            secretKeyRef:
              name: gentralretien-oauth
              key: client-secret
        volumeMounts:
        - name: data
          mountPath: /app/data

      volumes:
      - name: data
        persistentVolumeClaim:
          claimName: gentralretien-pvc
```

**Déployer:**
```bash
kubectl apply -f deployment.yaml
kubectl get secrets -n production
```

---

### 5️⃣ Enterprise (Vault / AWS)

**HashiCorp Vault:**
```csharp
// Installer: VaultSharp NuGet package
var client = new VaultClient(new VaultClientSettings("https://vault.example.com", authMethod));
var secret = await client.V1.Secrets.KeyValue.ReadSecretAsync("secret/data/gentralretien");
```

**AWS Secrets Manager:**
```csharp
// Installer: AWSSDK.SecretsManager
var client = new Amazon.SecretsManager.AmazonSecretsManagerClient();
var request = new GetSecretValueRequest { SecretId = "gentralretien" };
var response = await client.GetSecretValueAsync(request);
var secret = JsonConvert.DeserializeObject<Secret>(response.SecretString);
```

**Azure KeyVault:**
```csharp
// Installer: Azure.Security.KeyVault.Secrets
var client = new SecretClient(new Uri("https://yourvault.vault.azure.net/"), credential);
KeyVaultSecret secret = await client.GetSecretAsync("GoogleClientId");
```

---

## 🚀 GitHub Actions (CI/CD)

**Ajouter secrets au repository:**

1. GitHub → Settings → Secrets and variables → Actions
2. New repository secret:
   - `GOOGLE_CLIENT_ID`
   - `GOOGLE_CLIENT_SECRET`

**Workflow (`.github/workflows/deploy.yml`):**
```yaml
name: Deploy

on: [push]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Build & Push Docker Image
      run: |
        docker build -t ghcr.io/${{ github.repository }}:latest .
        echo ${{ secrets.GITHUB_TOKEN }} | docker login ghcr.io -u ${{ github.actor }} --password-stdin
        docker push ghcr.io/${{ github.repository }}:latest
    
    - name: Deploy
      env:
        GOOGLE_CLIENT_ID: ${{ secrets.GOOGLE_CLIENT_ID }}
        GOOGLE_CLIENT_SECRET: ${{ secrets.GOOGLE_CLIENT_SECRET }}
      run: |
        docker-compose -f docker-compose.prod.yml up -d
```

---

## 📋 Checklist Production

- [ ] Google credentials créés et copiés
- [ ] Secrets configurés dans le gestionnaire (Vault, K8s, AWS, etc.)
- [ ] `.env` et `appsettings.*.json` dans `.gitignore`
- [ ] HTTPS/SSL configuré
- [ ] Database backups planifiés
- [ ] Monitoring et alertes configurés
- [ ] Logs centralisés (ELK, Splunk, etc.)
- [ ] Health checks implémentés
- [ ] Resource limits définis
- [ ] Non-root user dans Docker

---

## 🎯 Résumé par Environnement

| Environnement | Méthode | Sécurité | Complexité |
|---|---|---|---|
| Local Dev | User Secrets | ✅✅✅ | ⭐ |
| Docker Local | .env file | ✅✅ | ⭐ |
| Swarm | Docker Secrets | ✅✅✅ | ⭐⭐ |
| Kubernetes | K8s Secrets | ✅✅✅ | ⭐⭐ |
| Enterprise | Vault | ✅✅✅✅ | ⭐⭐⭐ |

---

## 🔗 Ressources

- [Kubernetes Secrets](https://kubernetes.io/docs/concepts/configuration/secret/)
- [Docker Secrets](https://docs.docker.com/engine/swarm/secrets/)
- [HashiCorp Vault](https://www.vaultproject.io/)
- [AWS Secrets Manager](https://aws.amazon.com/secrets-manager/)
- [Azure KeyVault](https://azure.microsoft.com/en-us/services/key-vault/)
- [OWASP Secrets Management](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html)

---

**Résumé:** Utilisez le gestionnaire de secrets approprié à votre infrastructure. JAMAIS en plaintext ou dans git.
