# Versioning & Release

This guide explains how to manage versions and releases for GEntretien.

## Version Configuration

The application version is configured in the project file at [GEntretien/GEntretien.csproj](../../GEntretien/GEntretien.csproj):

```xml
<Version>1.0.0.0</Version>
<InformationalVersion>1.0.0.0</InformationalVersion>
```

Update both properties when preparing a new release.

## Displaying Version

The application version is automatically displayed in the header of the website as a blue badge (e.g., "v1.0.0.0").

This is handled by:
- [VersionService.cs](../../GEntretien/Application/Services/VersionService.cs) - Reads version from assembly attributes
- [MainLayout.razor](../../GEntretien/Components/Layout/MainLayout.razor) - Displays version badge in the header
- [MainLayout.razor.css](../../GEntretien/Components/Layout/MainLayout.razor.css) - Styles the version badge

## Release Process

### Step 1: Update Version in Code

Edit [GEntretien/GEntretien.csproj](../../GEntretien/GEntretien.csproj) and update the version numbers:

```xml
<Version>1.0.1.0</Version>
<InformationalVersion>1.0.1.0</InformationalVersion>
```

### Step 2: Commit Changes

Commit your changes (tests, new features, documentation, etc.):

```bash
git add .
git commit -m "Release 1.0.1.0: Description of changes"
```

### Step 3: Create a Git Tag

Create a tag matching the version pattern `v*`:

```bash
# For version 1.0.1.0
git tag v1.0.1.0

# Annotated tag (recommended for releases)
git tag -a v1.0.1.0 -m "Release version 1.0.1.0"
```

### Step 4: Push the Tag

Push the tag to trigger the CI/CD pipeline:

```bash
git push origin v1.0.1.0
```

Or push all tags at once:

```bash
git push origin --tags
```

## CI/CD Trigger

The GitHub Actions workflow (`.github/workflows/docker-ci.yml`) is configured to trigger **only when you push a tag matching the pattern `v*`**.

When you push a tag:
1. Tests are executed
2. Docker image is built
3. Image is pushed to GitHub Container Registry with the tag as the version

### Docker Image Tags

When you push tag `v1.0.1.0`, the Docker image will be tagged with:
- `ghcr.io/yourusername/GEntretien:1.0.1.0` (exact version)
- `ghcr.io/yourusername/GEntretien:1.0` (major.minor)
- `ghcr.io/yourusername/GEntretien:latest` (if it's the default branch's latest)

## Example Workflow

```bash
# 1. Make changes and test locally
npm run dev
# ... test the application ...

# 2. Update version in csproj
# Edit GEntretien/GEntretien.csproj
# <Version>1.1.0.0</Version>

# 3. Commit the version update
git add GEntretien/GEntretien.csproj
git commit -m "Bump version to 1.1.0.0"

# 4. Tag the commit
git tag -a v1.1.0.0 -m "Release v1.1.0.0 - New features and improvements"

# 5. Push tag to trigger CI/CD
git push origin v1.1.0.0

# 6. Watch GitHub Actions build and push the image
# Navigate to: https://github.com/yourusername/GEntretien/actions
```

## Version Format

Use semantic versioning: `MAJOR.MINOR.PATCH.BUILD`

Examples:
- `1.0.0.0` - Initial release
- `1.1.0.0` - New features (minor version bump)
- `1.0.1.0` - Bug fixes (patch version bump)
- `2.0.0.0` - Breaking changes (major version bump)

## Troubleshooting

### CI/CD didn't trigger

Make sure:
1. Tag name starts with `v` (e.g., `v1.0.0.0`, not `1.0.0.0`)
2. Tag is pushed to origin: `git push origin v1.0.0.0`
3. Check GitHub Actions tab for workflow runs

### Version not updating on website

1. Rebuild the project: `dotnet build`
2. Restart the application
3. Clear browser cache (Ctrl+Shift+Delete or Cmd+Shift+Delete)
4. Hard refresh the page (Ctrl+F5 or Cmd+Shift+R)

### Want to delete a tag

```bash
# Delete locally
git tag -d v1.0.0.0

# Delete from remote
git push origin --delete v1.0.0.0
```

## Additional Resources

- [Git Tagging Documentation](https://git-scm.com/book/en/v2/Git-Basics-Tagging)
- [Semantic Versioning](https://semver.org/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Image Tags](https://docs.docker.com/engine/reference/commandline/tag/)
