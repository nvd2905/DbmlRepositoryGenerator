param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$Message = ""
)

# Validate version format
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Error "Version must be in format: x.y.z (e.g., 1.0.0)"
    exit 1
}

Write-Host "🚀 Starting release process for version $Version" -ForegroundColor Green

# Update version in csproj file
$csprojPath = "DbmlRepositoryGenerator.csproj"
$csprojContent = Get-Content $csprojPath -Raw
$csprojContent = $csprojContent -replace '<Version>.*?</Version>', "<Version>$Version</Version>"
Set-Content $csprojPath $csprojContent

Write-Host "✅ Updated version in $csprojPath" -ForegroundColor Green

# Build and test
Write-Host "🔨 Building project..." -ForegroundColor Yellow
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

Write-Host "✅ Build successful" -ForegroundColor Green

# Pack
Write-Host "📦 Creating NuGet package..." -ForegroundColor Yellow
dotnet pack --configuration Release --output ./nupkg
if ($LASTEXITCODE -ne 0) {
    Write-Error "Pack failed!"
    exit 1
}

Write-Host "✅ Package created successfully" -ForegroundColor Green

# Git operations
Write-Host "📝 Committing changes..." -ForegroundColor Yellow
git add .
git status
if (-not (git diff --cached --quiet)) {
    git commit -m "Release version $Version" -m "$Message"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Commit failed!"
        exit 1
    }
} else {
    Write-Host "⚠ No changes to commit"
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Commit failed!"
    exit 1
}

Write-Host "🏷️ Creating tag v$Version..." -ForegroundColor Yellow
git tag "v$Version"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Tag creation failed!"
    exit 1
}

Write-Host "📤 Pushing changes and tag..." -ForegroundColor Yellow
git push origin main
git push origin "v$Version"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Push failed!"
    exit 1
}

Write-Host "🎉 Release v$Version completed successfully!" -ForegroundColor Green
Write-Host "The GitHub Action will now automatically publish to NuGet." -ForegroundColor Cyan 