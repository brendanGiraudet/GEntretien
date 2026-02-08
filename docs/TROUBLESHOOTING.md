# 🆘 Dépannage

Erreurs courantes et solutions.

---

## Port en Utilisation

### Windows
```powershell
netstat -ano | findstr :5000
# Cherchez le PID

taskkill /PID <PID> /F
```

### Linux/Mac
```bash
lsof -i :5000
kill -9 <PID>
```

---

## Erreurs d'Authentification

### "Invalid Client ID"
- Vérifier **User Secrets:** `dotnet user-secrets list`
- Vérifier que Client ID n'a pas de typos
- Vérifier que credentials viennent de Google Cloud Console

### "Signature validation failed"
- Client Secret incorrect ou avec typos
- Obtenir une nouvelle paire de credentials

### "Redirect URI mismatch"
- Ajouter `http://localhost:5000/signin-google` à Authorized Redirect URIs
- Ajouter `http://localhost:5001/signin-google` si HTTPS utilisé

### "Google+ API not enabled"
- Google Cloud Console → APIs & Services → Library
- Chercher "Google+ API"
- Cliquer ENABLE

---

## Database

### "database is locked"
```bash
# Fermer l'app (Ctrl+C)
# Supprimer le fichier database
rm GEntretien/app.db

# Relancer
dotnet run
```

### "No migrations applied"
```bash
cd GEntretien
dotnet ef database update -p . -s .
```

---

## Build Errors

### "NuGet package restore failed"
```bash
dotnet restore
dotnet clean
dotnet build
```

### "Microsoft.AspNetCore.Components.Authorization not found"
- Vérifier `GEntretien.csproj`
- Vérifier que packages sont bien installés: `dotnet restore`

### "CS1061: '... does not contain a definition for ...'"
- Ajouter les `@using` directives manquantes en haut du fichier Razor

---

## Docker

### "Cannot find image"
```bash
docker-compose up -d --build
```

### "Port already in use"
```bash
docker-compose down

# Ou sur un port différent
docker run -p 5001:5000 gentralretien
```

### "Environment variables not reading"
```bash
# Vérifier que .env existe
ls -la .env

# Vérifier contenu
cat .env

# Relancer
docker-compose up -d --remove-orphans
```

---

## Runtime Errors

### "No service for type X registered"
- Vérifier `Program.cs` - ajouter `builder.Services.AddScoped<X>()`

### "NullReferenceException"
- Vérifier que services sont injectés via constructor
- Vérifier que `@inject ServiceType Service` est utilisé en Razor

### "Cannot connect to database"
- Vérifier `appsettings.json` connectionString
- Vérifier que chemin vers `app.db` est correct
- Vérifier permissions fichier

---

## Performance

### "Application is slow"
1. Vérifier les requêtes SQL dans logs
2. Vérifier `DbContext` lazy loading vs eager loading
3. Vérifier que pagination est implémentée

### "Memory usage high"
1. Vérifier que sessions sont nettoyées
2. Vérifier que queries ne chargent pas toute la DB

---

## Segments de Log à Chercher

```
// OK ✅
"Now listening on: http://[::]:5000"
"Application started"

// Problèmes ⚠️
"error" (case-insensitive)
"failed" (case-insensitive)
"exception"
"unauthorized"
"signature"
"connection"
```

---

## Debugging

### Lancer en Debug Mode
```bash
dotnet run -c Debug --project GEntretien/
```

### Attacher Debugger VS Code
1. F5 (Start Debugging)
2. Sélectionner ".NET"
3. Sélectionner "gentralretien" ou le process

### Logs Détaillés
```bash
# Dans Program.cs
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Puis observer les erreurs
dotnet run
```

---

## Reset Complet

Si tout est cassé:

```bash
# Supprimer artifacts
rm -r GEntretien/bin GEntretien/obj GEntretien.Tests/bin GEntretien.Tests/obj

# Supprimer database
rm GEntretien/app.db*

# Supprimer secrets
cd GEntretien
dotnet user-secrets clear

# Restaurer
dotnet restore
dotnet build

# Reconfigurer secrets
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_SECRET"

# Relancer
dotnet run
```

---

## 📞 Où Chercher

1. **Logs console** — Premiers indices
2. **Browser console** (F12) — Erreurs client
3. **VS Code Debug** — Breakpoints et stacktrace
4. [GETTING_STARTED.md](GETTING_STARTED.md) — Quick start
5. [GOOGLE_OAUTH.md](GOOGLE_OAUTH.md) — OAuth specific
6. [../deployment/DOCKER.md](../deployment/DOCKER.md) — Docker issues

---

**Pas trouvé votre problème?** Créez une issue ou consultez la documentation complète.
