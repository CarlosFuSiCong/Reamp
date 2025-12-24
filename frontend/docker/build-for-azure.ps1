# Build script for Azure Container Apps deployment
# This script builds the frontend image with production environment variables

param(
    [Parameter(Mandatory=$true)]
    [string]$BackendApiUrl,
    
    [Parameter(Mandatory=$true)]
    [string]$FrontendAppUrl,
    
    [string]$GoogleMapsApiKey = "",
    
    [string]$AcrRegistry = "reampacrsicong.azurecr.io",
    
    [string]$ImageName = "reamp-frontend",
    
    [string]$Tag = "latest"
)

$FullImageName = "$AcrRegistry/${ImageName}:$Tag"

Write-Host "Building frontend Docker image for Azure..." -ForegroundColor Green
Write-Host "Backend API URL: $BackendApiUrl" -ForegroundColor Cyan
Write-Host "Frontend App URL: $FrontendAppUrl" -ForegroundColor Cyan
if ($GoogleMapsApiKey) {
    Write-Host "Google Maps API Key: [CONFIGURED]" -ForegroundColor Cyan
} else {
    Write-Host "Google Maps API Key: [NOT CONFIGURED]" -ForegroundColor Yellow
}
Write-Host "Image: $FullImageName" -ForegroundColor Cyan
Write-Host ""

# Navigate to frontend directory
Push-Location (Join-Path $PSScriptRoot "..")

try {
    # Build the image with build arguments
    $buildArgs = @(
        "--build-arg", "NEXT_PUBLIC_API_URL=$BackendApiUrl",
        "--build-arg", "NEXT_PUBLIC_APP_URL=$FrontendAppUrl",
        "--build-arg", "NEXT_PUBLIC_ENABLE_DEBUG=false"
    )
    
    if ($GoogleMapsApiKey) {
        $buildArgs += "--build-arg", "NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=$GoogleMapsApiKey"
    }
    
    $buildArgs += "-t", $FullImageName, "-f", "docker/Dockerfile", "."
    
    & docker build @buildArgs
    
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

