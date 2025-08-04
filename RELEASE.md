# Release Guide

This guide explains how to release a new version of DbmlRepositoryGenerator.

## Prerequisites

1. **NuGet API Key**: Get your API key from [NuGet.org](https://www.nuget.org/account/apikeys)
2. **GitHub Secrets**: Add your NuGet API key to GitHub repository secrets:
   - Go to your repository → Settings → Secrets and variables → Actions
   - Add a new secret named `NUGET_API_KEY` with your NuGet API key

## Automated Release Process

### Option 1: Using the Release Script (Recommended)

1. **Update your code** and commit your changes
2. **Run the release script**:
   ```powershell
   .\scripts\release.ps1 -Version "1.0.1" -Message "Fixed DI container issue"
   ```
3. **The script will**:
   - Update the version in `DbmlRepositoryGenerator.csproj`
   - Build and test the project
   - Create a NuGet package
   - Commit changes and create a git tag
   - Push to GitHub
   - Trigger the GitHub Action to publish to NuGet

### Option 2: Manual Release

1. **Update version** in `DbmlRepositoryGenerator.csproj`:
   ```xml
   <Version>1.0.1</Version>
   ```

2. **Commit and tag**:
   ```bash
   git add .
   git commit -m "Release version 1.0.1"
   git tag v1.0.1
   git push origin main
   git push origin v1.0.1
   ```

3. **GitHub Action will automatically**:
   - Build the project
   - Create the NuGet package
   - Publish to NuGet.org
   - Create a GitHub release

## Versioning

Follow [Semantic Versioning](https://semver.org/):
- **MAJOR.MINOR.PATCH** (e.g., 1.0.0)
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

## GitHub Actions Workflows

### `nuget-publish.yml`
- **Triggers**: Only on version tags (v*.*.*)
- **Purpose**: Simple publish workflow

### `ci-cd.yml`
- **Triggers**: On pushes to main/develop, PRs, and version tags
- **Purpose**: Full CI/CD pipeline with testing, building, and publishing

## Troubleshooting

### Build Fails
- Check that all dependencies are properly referenced
- Ensure the project builds locally before pushing

### Publish Fails
- Verify your `NUGET_API_KEY` secret is set correctly
- Check that the version number is unique (not already published)
- Ensure the package metadata is correct in `.csproj`

### GitHub Action Not Triggered
- Make sure you're pushing a tag that matches the pattern `v*.*.*`
- Check the Actions tab in your GitHub repository for workflow runs

## Manual Publishing (if needed)

If you need to publish manually:

```bash
# Build and pack
dotnet pack --configuration Release --output ./nupkg

# Publish to NuGet
dotnet nuget push ./nupkg/DbmlRepositoryGenerator.1.0.1.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

## Testing Before Release

Test your package locally before releasing:

```bash
# Install locally
dotnet tool install --global --add-source ./nupkg DbmlRepositoryGenerator

# Test the tool
dbml-repo-gen --help

# Uninstall test version
dotnet tool uninstall --global DbmlRepositoryGenerator
``` 