# 🏗️ Architecture et Structure du Projet

---

## 🎯 Vue d'ensemble

GEntretien est construit avec une **Clean Architecture** à 4 couches:

```
┌─────────────────────────────────────┐
│      Web / User Interface           │  ← Blazor Server Components
│  (GEntretien/Components)            │
├─────────────────────────────────────┤
│    Application / Business Logic     │  ← DTOs, Validators, Services
│  (GEntretien/Application)           │
├─────────────────────────────────────┤
│    Infrastructure / Data Access     │  ← EF Core, Repositories
│  (GEntretien/Infrastructure)        │
├─────────────────────────────────────┤
│       Domain / Core Entities        │  ← Business Rules (No Dependencies)
│    (GEntretien/Domain)              │
└─────────────────────────────────────┘
```

### Avantages
- ✅ **Testabilité** — Core logic sans dépendances externes
- ✅ **Maintenabilité** — Code clair et organisé
- ✅ **Flexibilité** — Facile de changer les détails (BDD, ORM, UI)
- ✅ **Réutilisabilité** — Logique métier indépendante du framework

---

## 📁 Structure des Dossiers

```
GEntretien/
├── Domain/                          # ⭐ Entités métier pures
│   ├── Entities/
│   │   └── Equipment.cs             # (Maintenez-le simple, pas de dépendances)
│   └── Interfaces/
│       └── IEquipmentRepository.cs  # Contrats d'accès aux données
│
├── Application/                     # 📦 Use cases + Validation
│   ├── Validators/
│   │   └── EquipmentValidator.cs    # FluentValidation rules
│   └── (DTOs, Services, use-cases)
│
├── Infrastructure/                  # 🗄️ Détails techniques
│   ├── Persistence/
│   │   ├── AppDbContext.cs          # EF Core DbContext
│   │   └── Migrations/
│   └── Repositories/
│       └── EquipmentRepository.cs   # CRUD implementation
│
├── Components/                      # 🎨 UI Blazor Server
│   ├── App.razor                    # Root component + Auth wrapper
│   ├── Routes.razor                 # Routing + Protection
│   ├── Pages/
│   │   ├── Home.razor
│   │   ├── Login.razor              # ✨ Google OAuth
│   │   ├── Counter.razor
│   │   └── Error.razor
│   └── Layout/
│       ├── MainLayout.razor
│       └── NavMenu.razor            # ✨ User dropdown + Logout
│
├── Program.cs                       # 🚀 DI + Middleware setup
├── appsettings.json                 # ⚙️ Configuration
└── wwwroot/                         # 📚 Static files
```

---

## 🧬 Principes de Conception

### SOLID

| Principe | Application |
|----------|-------------|
| **S**ingle Responsibility | Chaque classe a une seule raison de changer |
| **O**pen/Closed | Classes ouvertes à l'extension, fermées à la modification |
| **L**iskov Substitution | Implémentations interchangeables |
| **I**nterface Segregation | Interfaces spécialisées (IEquipmentRepository) |
| **D**ependency Inversion | DI injection via constructor, interfaces abstraites |

### KISS (Keep It Simple, Stupid)

- Code clair et lisible
- Pas d'abstraction inutile
- Pas de patterns overcompliqués

### YAGNI (You Aren't Gonna Need It)

- Implémenter features quand demandées
- Ne pas anticiper des besoins futurs

### Boy Scout Rule

- Laisser le code plus propre qu'on l'a trouvé
- Refactorisations continues

---

## 🔐 Authentification OAuth 2.0

### Flow Complet

```
User visits /
  ↓
Middleware checks auth
  ↓
No session → Redirect /login
  ↓
Login.razor displays
  ↓
User clicks "Se connecter avec Google"
  ↓
POST /login endpoint
  ↓
context.ChallengeAsync(GoogleDefaults.AuthenticationScheme)
  ↓
Redirect to Google OAuth
  ↓
User authenticates at Google
  ↓
Google redirects to /signin-google
  ↓
Cookie auth scheme creates session
  ↓
Redirect to /
  ↓
AuthorizeRouteView allows access
  ↓
✅ AUTHENTICATED
```

### Configuration dans Program.cs

```csharp
// Services
builder.Services.AddAuthentication(...)
    .AddCookie(...)
    .AddGoogle(...);
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

// Middleware
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapGet("/login", ...);   // Google OAuth challenge
app.MapPost("/logout", ...); // SignOut
```

---

## 📊 Data Flow

### CRUD Équipement

```
1. User fills form
   ↓
2. Form validation (FluentValidationValidator)
   ↓
3. Submit → EquipmentEdit.razor
   ↓
4. Call IEquipmentRepository
   ↓
5. Repository calls DbContext
   ↓
6. EF Core generates SQL
   ↓
7. SQLite executes
   ↓
8. Response → UI Update
   ↓
9. Success message
```

### Validation

**EquipmentValidator.cs**
```csharp
RuleFor(x => x.Name)
    .NotEmpty().WithMessage("Requis")
    .MaximumLength(200).WithMessage("Trop long");
```

**UI (EquipmentEdit.razor)**
```razor
<FluentValidationValidator/>
<ValidationSummary/>
<InputText @bind-Value="Equipment.Name" />
```

---

## 🧪 Testing

### Unit Tests (xUnit + FluentValidation.TestHelper)

```csharp
[Fact]
public void Should_fail_when_name_empty()
{
    var model = new Equipment { Name = "" };
    var result = new EquipmentValidator().Validate(model);
    result.IsValid.Should().BeFalse();
}
```

### Test Locations
- `GEntretien.Tests/Validators/` — Validator tests
- Run: `dotnet test`

---

## 🚀 Déploiement

### Localement
```bash
dotnet run --project GEntretien/
```

### Docker
```bash
docker-compose up -d
# Ou avec script: .\scripts\DOCKER_DEPLOY.ps1 up
```

### Production
Voir [../deployment/DOCKER.md](../deployment/DOCKER.md) et [../deployment/SECRETS_MANAGEMENT.md](../deployment/SECRETS_MANAGEMENT.md)

---

## 📈 Évolution Future

**Domain Models à ajouter:**
- MaintenanceRecord (historique d'entretiens)
- Location (localisations)
- MaintenanceSchedule (planning)
- User (stockage des utilisateurs Google)

**Patterns à implémenter:**
- MediatR pour les use cases
- CQRS si complexité augmente
- Event sourcing pour audit

Mais pour l'instant: **KISS** ✨

---

## 🔗 Ressources

- [.NET 10 Architecture](https://learn.microsoft.com/en-us/dotnet/10)
- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Blazor Server Docs](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
