# Push script for Azure Container Registry
# This script pushes the frontend image to ACR

param(
    [string]$AcrRegistry = "reampacrsicong.azurecr.io",
    [string]$ImageName = "reamp-frontend",
    [string]$Tag = "latest"
)

$FullImageName = "$AcrRegistry/${ImageName}:$Tag"

Write-Host "Pushing frontend image to ACR..." -ForegroundColor Green
Write-Host "Image: $FullImageName" -ForegroundColor Cyan
Write-Host ""

docker push $FullImageName

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Push successful!" -ForegroundColor Green
    Write-Host "Image is now available at: $FullImageName" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "To deploy to Azure Container Apps, run:" -ForegroundColor Yellow
    Write-Host "  az containerapp update \" -ForegroundColor White
    Write-Host "    --name reamp-frontend \" -ForegroundColor White
    Write-Host "    --resource-group <your-rg> \" -ForegroundColor White
    Write-Host "    --image $FullImageName" -ForegroundColor White
} else {
    Write-Host "Push failed!" -ForegroundColor Red
    exit 1
}

