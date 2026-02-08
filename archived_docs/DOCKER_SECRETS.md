# Guide de Déploiement Docker avec Google OAuth - GEntretien

## 📋 Table des matières

1. [Développement Local](#développement-local)
2. [Déploiement Docker](#déploiement-docker)
3. [Production avec Secrets](#production-avec-secrets)
4. [Kubernetes](#kubernetes)
5. [Troubleshooting](#troubleshooting)

---

## Développement Local

### Avec docker-compose et variables d'environnement

**Étape 1: Créer un fichier `.env` local**

```bash
cp .env.example .env
```

Éditer `.env` et ajouter vos vraies credentials:

```env
GOOGLE_CLIENT_ID=YOUR_CLIENT_ID_HERE
GOOGLE_CLIENT_SECRET=YOUR_CLIENT_SECRET_HERE
ASPNETCORE_ENVIRONMENT=Development
```

**Étape 2: Lancer docker-compose**

```bash
docker-compose up -d
```

Les variables depuis `.env` seront automatiquement chargées dans le container.

**Étape 3: Vérifier les logs**

```bash
docker-compose logs -f gentralretien
```

**Étape 4: Arrêter**

```bash
docker-compose down
```

---

## Déploiement Docker

### Build manuel

```bash
# Build avec nom personnalisé
docker build -t gentralretien:1.0.0 .

# Lancer le container
docker run -p 5000:5000 \
  -e Authentication__Google__ClientId=YOUR_CLIENT_ID \
  -e Authentication__Google__ClientSecret=YOUR_CLIENT_SECRET \
  -e ASPNETCORE_ENVIRONMENT=Production \
  gentralretien:1.0.0
```

### Push vers registre (GitHub Container Registry)

```bash
# Login
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Tag image
docker tag gentralretien:1.0.0 ghcr.io/USERNAME/gentralretien:1.0.0
docker tag gentralretien:1.0.0 ghcr.io/USERNAME/gentralretien:latest

# Push
docker push ghcr.io/USERNAME/gentralretien:1.0.0
docker push ghcr.io/USERNAME/gentralretien:latest
```

---

## Production avec Secrets

### ⚠️ Important : Gestion Sécurisée des Secrets

**🚩 JAMAIS:**
- ❌ Hardcoder les secrets dans le Dockerfile
- ❌ Passer les secrets en clair en ligne de commande (visible dans `ps aux`)
- ❌ Commiter `.env` dans git
- ❌ Stocker en plaintext

**✅ À FAIRE:**
1. Utiliser `Docker Secrets` (Swarm) ou `Kubernetes Secrets`
2. Utiliser des gestionnaires de secrets (Vault, AWS Secrets Manager, etc.)
3. Utiliser des variables d'environnement sécurisées (Azure KeyVault, etc.)

### Option 1 : Docker Secrets (Docker Swarm)

**Créer les secrets:**

```bash
# Créer les secrets une seule fois
echo "YOUR_GOOGLE_CLIENT_ID" | docker secret create google_client_id -
echo "YOUR_GOOGLE_CLIENT_SECRET" | docker secret create google_client_secret -
```

**Fichier `docker-compose.prod.yml`:**

```yaml
version: '3.8'

services:
  gentralretien:
    image: ghcr.io/USERNAME/gentralretien:latest
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000
      - Authentication__Google__ClientId_FILE=/run/secrets/google_client_id
      - Authentication__Google__ClientSecret_FILE=/run/secrets/google_client_secret
    secrets:
      - google_client_id
      - google_client_secret
    restart: unless-stopped
    networks:
      - gentralretien-network

secrets:
  google_client_id:
    external: true
  google_client_secret:
    external: true

networks:
  gentralretien-network:
    driver: overlay
```

**Déployer sur Swarm:**

```bash
docker stack deploy -c docker-compose.prod.yml gentralretien
```

**Lire les secrets dans l'application:**

```csharp
// Program.cs
var googleClientId = File.ReadAllText(
    builder.Configuration["Authentication:Google:ClientId"] ?? 
    "/run/secrets/google_client_id"
).Trim();

var googleClientSecret = File.ReadAllText(
    builder.Configuration["Authentication:Google:ClientSecret"] ?? 
    "/run/secrets/google_client_secret"
).Trim();

builder.Services
    .AddAuthentication(options => { ... })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        // ... rest of config
    });
```

### Option 2 : Fichier Secrets Signé (Avancé)

Pour une sécurité maximale, utilisez user secrets dans un fichier signé:

```bash
# Générer une clé
openssl rand -base64 32 > /secure/location/secret.key

# Chiffrer les secrets
gpg --symmetric --cipher-algo AES256 \
    --output secrets.enc \
    --passphrase "$(cat /secure/location/secret.key)" \
    secrets.txt

# Le Dockerfile déchiffre au démarrage
```

---

## Kubernetes

### Créer un secret Kubernetes

```bash
kubectl create secret generic gentralretien-secrets \
  --from-literal=google_client_id=YOUR_CLIENT_ID \
  --from-literal=google_client_secret=YOUR_CLIENT_SECRET \
  -n production
```

### Fichier `k8s-deployment.yaml`

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gentralretien
  namespace: production
spec:
  replicas: 3
  selector:
    matchLabels:
      app: gentralretien
  template:
    metadata:
      labels:
        app: gentralretien
    spec:
      containers:
        - name: gentralretien
          image: ghcr.io/USERNAME/gentralretien:latest
          imagePullPolicy: IfNotPresent
          ports:
            - containerPort: 5000
          env:
            - name: ASPNETCORE_ENVIRONMENT
              value: Production
            - name: Authentication__Google__ClientId
              valueFrom:
                secretKeyRef:
                  name: gentralretien-secrets
                  key: google_client_id
            - name: Authentication__Google__ClientSecret
              valueFrom:
                secretKeyRef:
                  name: gentralretien-secrets
                  key: google_client_secret
          resources:
            requests:
              memory: "256Mi"
              cpu: "250m"
            limits:
              memory: "512Mi"
              cpu: "500m"
          livenessProbe:
            httpGet:
              path: /health
              port: 5000
            initialDelaySeconds: 30
            periodSeconds: 10
          readinessProbe:
            httpGet:
              path: /health
              port: 5000
            initialDelaySeconds: 10
            periodSeconds: 5
          volumeMounts:
            - name: data
              mountPath: /app/data
      volumes:
        - name: data
          persistentVolumeClaim:
            claimName: gentralretien-pvc

---
apiVersion: v1
kind: Service
metadata:
  name: gentralretien-service
  namespace: production
spec:
  type: LoadBalancer
  selector:
    app: gentralretien
  ports:
    - protocol: TCP
      port: 80
      targetPort: 5000

---
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: gentralretien-pvc
  namespace: production
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 10Gi
```

**Déployer:**

```bash
kubectl apply -f k8s-deployment.yaml
kubectl get pods -n production
kubectl logs -f deployment/gentralretien -n production
```

---

## Troubleshooting

### "Invalid Client ID" en production

```bash
# Vérifier que les secrets sont bien montés
docker exec gentralretien cat /run/secrets/google_client_id

# Vérifier les logs
docker logs gentralretien | grep -i "authentication\|google"
```

### Permissions refusées (500 Internal Server Error)

```bash
# Vérifier que l'utilisateur du container a les bonnes permissions
docker exec gentralretien ls -la /run/secrets/

# Le container tourne en tant que quel utilisateur?
docker inspect gentralretien | grep -i user
```

### Database locked

```bash
# SQLite peut avoir des problèmes avec volumes Docker
# Utiliser un volume bind ou une vraie base de données

# Option 1: Volume bind (développement)
docker run -v $(pwd)/data:/app/data gentralretien

# Option 2: PostgreSQL ou SQL Server (production)
# Mettre à jour la connection string dans appsettings.json
```

### HTTPS et redirection Google OAuth

En production, **HTTPS est obligatoire** pour Google OAuth:

```csharp
// Program.cs - Forcer HTTPS
app.UseHttpsRedirection();

// Ou en contexte de LoadBalancer/Reverse Proxy
app.Use((context, next) =>
{
    if (!context.Request.IsHttps && !context.Request.Host.Host.Contains("localhost"))
    {
        var httpsUrl = $"https://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(httpsUrl);
        return Task.CompletedTask;
    }
    return next();
});
```

### Reverse Proxy (Nginx/Apache)

**Nginx config:**

```nginx
server {
    listen 443 ssl http2;
    server_name yourdomain.com;

    ssl_certificate /etc/ssl/certs/cert.pem;
    ssl_certificate_key /etc/ssl/private/key.pem;

    location / {
        proxy_pass http://gentralretien:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto https;
    }

    # Update Google OAuth redirect URIs to include:
    # https://yourdomain.com/signin-google
}
```

---

## ✅ Checklist de Production

- [ ] Google OAuth credentials configurés dans un gestionnaire de secrets
- [ ] Docker image taggée avec version (pas `latest` en prod)
- [ ] HTTPS activé (certificat SSL/TLS)
- [ ] `.env` jamais commité; `.env.example` créé
- [ ] Health checks configurés (livenessProbe, readinessProbe)
- [ ] Logs centralisés (ELK, Splunk, CloudWatch, etc.)
- [ ] Backups de la base de données planifiés
- [ ] Monitoring et alertes (CPU, Memory, Erreurs)
- [ ] Rate limiting / Protection DDoS (Cloudflare, WAF, etc.)
- [ ] CORS configuré si API séparée

---

## 📚 Ressources

- [Docker Secrets Best Practices](https://docs.docker.com/engine/swarm/secrets/)
- [Kubernetes Secrets](https://kubernetes.io/docs/concepts/configuration/secret/)
- [ASP.NET Core Environment Variables](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Google OAuth 2.0 Redirect URIs](https://developers.google.com/identity/protocols/oauth2)
- [OWASP Secrets Management](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html)
