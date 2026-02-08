# 🧪 Tests

---

## Exécuter les Tests

```bash
# Tous les tests
dotnet test

# Avec output verbose
dotnet test --logger "console;verbosity=detailed"

# Spécifique au projet
dotnet test GEntretien.Tests/

# Spécifique au fichier
dotnet test GEntretien.Tests/ --filter "EquipmentValidatorTests"

# Avec coverage (si opencover/coverlet installé)
dotnet test /p:CollectCoverage=true
```

---

## Test Structure

```
GEntretien.Tests/
└── Validators/
    └── EquipmentValidatorTests.cs
```

---

## Exemples de Tests

### Validator Tests (FluentValidation)

```csharp
[Fact]
public void Should_fail_when_name_is_empty()
{
    var model = new Equipment { Name = "" };
    var result = new EquipmentValidator().Validate(model);
    
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == "Name");
}

[Fact]
public void Should_pass_when_valid()
{
    var model = new Equipment { Name = "Compresseur" };
    var result = new EquipmentValidator().Validate(model);
    
    Assert.True(result.IsValid);
}
```

### Repository Tests (Moq)

```csharp
[Fact]
public async Task GetByIdAsync_Should_Return_Equipment()
{
    // Arrange
    var mockRepo = new Mock<IEquipmentRepository>();
    mockRepo
        .Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Equipment { Id = 1, Name = "Test" });

    // Act
    var result = await mockRepo.Object.GetByIdAsync(1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}
```

---

## Frameworks

- **xUnit** — Test framework (.NET standard)
- **FluentValidation.TestHelper** — Validation assertions

---

## Best Practices

✅ **À faire:**
- Tests pour validators
- Tests pour repositories
- Tests pour services complexes
- Naming: Test_Scenario_ExpectedResult
- Arrange-Act-Assert pattern
- Mocking external dependencies

❌ **À éviter:**
- Tests UI (UI frameworks change)
- Tests avec vraie DB (utiliser mocks)
- Tests dépendants l'un de l'autre
- Tests sans noms clairs

---

## Test Driven Development (TDD)

```
1. Red: Écrire test qui échoue
2. Green: Écrire minimum code pour passer
3. Refactor: Améliorer le code
4. Repeat
```

---

## Coverage

```bash
# Avec Coverlet
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Générer rapport HTML
coverlet GEntretien.Tests/bin/Debug/net10.0/GEntretien.Tests.dll \
  --target dotnet \
  --targetargs "test GEntretien.Tests/" \
  --format html

# Ouvrir rapport
open CoverageReport/index.html  # Linux/Mac
Start CoverageReport/index.html # Windows
```

---

## Ci/CD Integration

Les tests s'exécutent automatiquement dans GitHub Actions (voir `.github/workflows/`).

```yaml
- name: Run Tests
  run: dotnet test --logger "trx;LogFileName=results.trx"
```

---

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://moq.github.io/moq/wiki/quickstart)
- [FluentValidation Testing](https://docs.fluentvalidation.net/latest/testing)
- [Test Driven Development](https://en.wikipedia.org/wiki/Test-driven_development)

---

Besoin d'aide? Consulter [docs/TROUBLESHOOTING.md](../docs/TROUBLESHOOTING.md).
