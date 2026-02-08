# GEntretien - Gestion d'Entretien de Matériel

Une application web de gestion d'entretien de matériel construite avec **Blazor .NET 10**, **SQLite**, et **Clean Architecture**.

## 🎯 Principes de Développement

- **SOLID** — Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **KISS** — Keep It Simple, Stupid
- **YAGNI** — You Aren't Gonna Need It
- **Boy Scout** — Leave code cleaner than you found it
- **Clean Architecture** — Separation of concerns (Domain, Application, Infrastructure, Web)
- **Feature Folders** — Organize code by feature, not by layer

## 🚀 Quick Start

### Prerequisites

- .NET 10 SDK ([Download](https://dotnet.microsoft.com/en-us/download/dotnet/10.0))
- Visual Studio 2022 / VS Code (optional)

### Local Development

```bash
# Clone repository
git clone <repo-url>
cd RepoTest

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run application
dotnet run --project GEntretien/GEntretien.csproj
```

Access the app at: `http://localhost:5000`

### With Docker

```bash
# Build and run with Docker Compose
docker-compose up -d

# View logs
docker-compose logs -f gentralretien

# Stop
docker-compose down
```

See [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md) for detailed Docker guide.

## 📁 Project Structure

```
GEntretien/
├── Domain/                          # Core business logic (no external dependencies)
│   ├── Entities/
│   │   └── Equipment.cs
│   └── Interfaces/
│       └── IEquipmentRepository.cs
├── Application/                     # Use cases, DTOs, validators
│   ├── Validators/
│   │   └── EquipmentValidator.cs
│   └── (DTOs, Services, use-cases)
├── Infrastructure/                  # EF Core, repositories, migrations
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/
│   └── Repositories/
│       └── EquipmentRepository.cs
├── Web/                             # Blazor Server UI (Feature Folders)
│   ├── Features/
│   │   ├── Equipment/
│   │   │   ├── Pages/
│   │   │   │   ├── EquipmentList.razor
│   │   │   │   └── EquipmentEdit.razor
│   │   │   └── Components/
│   │   └── Maintenance/
│   ├── Components/                  # Shared components
│   │   ├── App.razor
│   │   ├── Routes.razor
│   │   └── Layout/
│   ├── Program.cs
│   ├── appsettings.json
│   └── wwwroot/
├── GEntretien.Tests/                # Unit & Integration tests
│   └── Validators/
│       └── EquipmentValidatorTests.cs
├── Dockerfile                       # Multi-stage Docker build
├── docker-compose.yml              # Local Docker setup
└── README.md (this file)
```

## 🛠️ Development Workflow

### Adding a New Feature

1. **Create Domain Model** (if needed)
   - Add entity in `Domain/Entities/`
   - Add repository interface in `Domain/Interfaces/`

2. **Create Application Layer**
   - Add DTOs in `Application/`
   - Add validators in `Application/Validators/`
   - Add services/use-cases in `Application/Services/`

3. **Create Infrastructure**
   - Implement repository in `Infrastructure/Repositories/`
   - Update `AppDbContext` with `DbSet<T>`

4. **Create Feature Folder** (Web)
   - Create folder in `Web/Features/{FeatureName}/`
   - Add Pages, Components, Services as needed

5. **Register Services** in `Program.cs`
   ```csharp
   builder.Services.AddScoped<IMyRepository, MyRepository>();
   ```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test GEntretien.Tests/GEntretien.Tests.csproj

# With verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Database Migrations

```bash
# Create new migration
dotnet ef migrations add MigrationName -p GEntretien/GEntretien.csproj -s GEntretien/GEntretien.csproj

# Apply migrations
dotnet ef database update -p GEntretien/GEntretien.csproj -s GEntretien/GEntretien.csproj

# Remove last migration (if not applied yet)
dotnet ef migrations remove -p GEntretien/GEntretien.csproj -s GEntretien/GEntretien.csproj
```

## 🧪 Testing

The project uses:
- **xUnit** for test framework
- **FluentValidation.TestHelper** for validation tests

Example test:

```csharp
[Fact]
public void Should_have_error_when_name_is_empty()
{
    var model = new Equipment { Name = "" };
    var result = new EquipmentValidator().Validate(model);
    Assert.False(result.IsValid);
}
```

## 🔒 Validation

Uses **FluentValidation** with Blazored integration. Rules defined in `Application/Validators/`.

Example:

```csharp
RuleFor(x => x.Name)
    .NotEmpty().WithMessage("Le nom est requis")
    .MaximumLength(200).WithMessage("Le nom est trop long");
```

Applied in forms via `<FluentValidationValidator />` component.

## � Google OAuth Authentication

The entire application requires authentication via Google Sign-In. Users must authenticate before accessing any page.

### Quick Setup

#### Option 1: Automated Setup (PowerShell)
```powershell
.\SETUP_SECRETS.ps1
```
This script will:
1. Initialize user secrets if needed
2. Prompt for Google OAuth credentials
3. Securely store them locally

#### Option 2: Manual Setup
```bash
cd GEntretien

# Initialize user secrets (first time only)
dotnet user-secrets init

# Add Google credentials
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"

# Verify
dotnet user-secrets list
```

### Getting Google OAuth Credentials

See [GOOGLE_OAUTH_SETUP.md](GOOGLE_OAUTH_SETUP.md) for **detailed step-by-step guide**:
1. Create Google Cloud project
2. Enable Google+ API
3. Create OAuth 2.0 credentials
4. Configure authorized redirect URIs
5. Copy Client ID and Secret

**Quick checklist:**
- ✅ Authorized JavaScript Origins: `http://localhost:5000`, `http://localhost:5001`
- ✅ Authorized Redirect URIs: `http://localhost:5000/signin-google`, `http://localhost:5001/signin-google`
- ✅ Scopes: `email`, `profile`

### How It Works

1. **Unauthenticated users** → Redirected to `/login` page
2. **Click "Se connecter avec Google"** → Google OAuth flow
3. **Google sign-in** → Returns to app with authenticated session
4. **NavMenu displays** → User name, email, logout button
5. **Click "Déconnexion"** → Clears session, redirects to login

### Security Notes

- ⚠️ **Never commit** `appsettings.Development.json` or secrets
- `appsettings.*.json` is in `.gitignore` (secrets protected)
- User secrets stored locally: `%APPDATA%\Microsoft\UserSecrets\` (Windows) or `~/.microsoft/usersecrets/` (Linux/Mac)
- Production: Use environment variables or Docker Secrets

## �🐳 Docker & CI/CD

### Automated Pipeline

GitHub Actions workflow automatically:
1. ✅ Runs tests on PR/push
2. 🏗️ Builds Docker image
3. 📦 Pushes to GitHub Container Registry (ghcr.io)

See [.github/workflows/docker-ci.yml](.github/workflows/docker-ci.yml) for details.

### Manual Docker Build

```bash
docker build -t gentralretien:latest .
docker run -p 5000:5000 gentralretien:latest
```

## 📋 Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "Default": "Data Source=app.db"
  }
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` — `Development` or `Production`
- `ASPNETCORE_URLS` — e.g., `http://+:5000`
- `ConnectionStrings__Default` — Database connection string

## 🚢 Deployment

See [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md) for:
- Kubernetes deployment
- Docker Swarm setup
- Traditional VM deployment
- Production database options

## 📚 Architecture Decisions

### Why Clean Architecture?

- **Testability** — Core business logic has no external dependencies
- **Maintainability** — Easy to understand and change code
- **Flexibility** — Can swap implementations (e.g., SQLite → PostgreSQL)
- **Scalability** — Clear separation makes adding features straightforward

### Domain Layer Benefits

- No references to EF Core, ASP.NET, or UI framework
- Reusable across different applications (web, API, CLI)
- Protected from external technology changes

### Repository Pattern

- Abstract data access from business logic
- Easier to test (in-memory repositories)
- Database-agnostic business logic

## 🐛 Troubleshooting

### Application won't start
```bash
# Check if port is in use
netstat -ano | findstr :5000

# Run with environment override
dotnet run --project GEntretien/GEntretien.csproj -- --environment Development
```

### Database migration fails
```bash
# Ensure database file location is correct
# Default: {project-root}/app.db

# Check EF Console setup
dotnet ef --help
```

### Docker image build fails
- Check `Dockerfile` paths match actual project structure
- Verify `.NET 10 SDK` support in Docker image
- Check `.dockerignore` isn't excluding required files

## 📞 Support & Contributing

1. Create an issue for bugs or feature requests
2. Follow the project structure and SOLID principles
3. Add tests for new code
4. Keep commits small and descriptive

## 📄 License

[Add your license here]

## 🔗 Useful Links

- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)
- [Blazor Server Guide](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
