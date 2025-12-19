# Build script for Azure Container Apps deployment
# This script builds the frontend image with production environment variables

param(
    [Parameter(Mandatory=$true)]
    [string]$BackendApiUrl,
    
    [Parameter(Mandatory=$true)]
    [string]$FrontendAppUrl,
    
    [string]$AcrRegistry = "reampacrsicong.azurecr.io",
    
    [string]$ImageName = "reamp-frontend",
    
    [string]$Tag = "latest"
)

$FullImageName = "$AcrRegistry/${ImageName}:$Tag"

Write-Host "Building frontend Docker image for Azure..." -ForegroundColor Green
Write-Host "Backend API URL: $BackendApiUrl" -ForegroundColor Cyan
Write-Host "Frontend App URL: $FrontendAppUrl" -ForegroundColor Cyan
Write-Host "Image: $FullImageName" -ForegroundColor Cyan
Write-Host ""

# Navigate to frontend directory
Push-Location (Join-Path $PSScriptRoot "..")

try {
    # Build the image with build arguments
    docker build `
        --build-arg NEXT_PUBLIC_API_URL=$BackendApiUrl `
        --build-arg NEXT_PUBLIC_APP_URL=$FrontendAppUrl `
        --build-arg NEXT_PUBLIC_ENABLE_DEBUG=false `
        -t $FullImageName `
        -f docker/Dockerfile `
        .
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Build successful!" -ForegroundColor Green
        Write-Host "Image: $FullImageName" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "To push to ACR, run:" -ForegroundColor Yellow
        Write-Host "  docker push $FullImageName" -ForegroundColor White
        Write-Host ""
        Write-Host "Or run:" -ForegroundColor Yellow
        Write-Host "  .\docker\push-to-azure.ps1" -ForegroundColor White
    } else {
        Write-Host "Build failed!" -ForegroundColor Red
        exit 1
    }
} finally {
    Pop-Location
}

