# Azure Backend Deployment Script
# This script deploys the backend API to Azure

param(
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroup = "reamp-rg",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "eastasia",
    
    [Parameter(Mandatory=$false)]
    [string]$AcrName = "reampacr",
    
    [Parameter(Mandatory=$false)]
    [string]$ContainerAppName = "reamp-api",
    
    [Parameter(Mandatory=$false)]
    [string]$ContainerAppEnv = "reamp-env",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlServerName = "",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlDatabaseName = "ReampDb",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory=$false)]
    [switch]$RunMigrations
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Reamp Backend Deployment to Azure" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if logged in to Azure
Write-Host "Checking Azure login..." -ForegroundColor Yellow
$azAccount = az account show 2>$null | ConvertFrom-Json
if (-not $azAccount) {
    Write-Host "Not logged in to Azure. Please login..." -ForegroundColor Yellow
    az login
    $azAccount = az account show | ConvertFrom-Json
}

Write-Host "Using subscription: $($azAccount.name)" -ForegroundColor Green
Write-Host ""

# Navigate to backend directory
$BackendPath = Join-Path $PSScriptRoot ".."
Push-Location $BackendPath

try {
    # Step 1: Build and push Docker image to ACR
    if (-not $SkipBuild) {
        Write-Host "Step 1: Building and pushing Docker image to ACR..." -ForegroundColor Cyan
        Write-Host "ACR Name: $AcrName" -ForegroundColor Gray
        Write-Host ""
        
        # Build and push using ACR build (builds in Azure)
        Write-Host "Building image in Azure Container Registry..." -ForegroundColor Yellow
        az acr build `
            --registry $AcrName `
            --image reamp-api:latest `
            --file docker/Dockerfile `
            .
        
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to build and push Docker image"
        }
        
        Write-Host "Docker image built and pushed successfully!" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host "Skipping Docker build (using existing image)" -ForegroundColor Yellow
        Write-Host ""
    }
    
    # Step 2: Update Container App
    Write-Host "Step 2: Updating Container App..." -ForegroundColor Cyan
    Write-Host "Container App: $ContainerAppName" -ForegroundColor Gray
    Write-Host ""
    
    # Check if container app exists
    $containerAppExists = az containerapp show `
        --name $ContainerAppName `
        --resource-group $ResourceGroup `
        2>$null
    
    if (-not $containerAppExists) {
        Write-Host "Container App does not exist. Please create it first using Azure Portal or az CLI." -ForegroundColor Red
        Write-Host "Example command:" -ForegroundColor Yellow
        Write-Host "az containerapp create --name $ContainerAppName --resource-group $ResourceGroup --environment $ContainerAppEnv --image $AcrName.azurecr.io/reamp-api:latest --target-port 8080 --ingress external" -ForegroundColor Gray
        exit 1
    }
    
    # Update container app with new image
    Write-Host "Updating container app with new image..." -ForegroundColor Yellow
    az containerapp update `
        --name $ContainerAppName `
        --resource-group $ResourceGroup `
        --image "$AcrName.azurecr.io/reamp-api:latest"
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to update Container App"
    }
    
    Write-Host "Container App updated successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Step 3: Run database migrations (optional)
    if ($RunMigrations) {
        Write-Host "Step 3: Running database migrations..." -ForegroundColor Cyan
        Write-Host ""
        
        if ([string]::IsNullOrEmpty($SqlServerName)) {
            Write-Host "Error: SqlServerName is required for migrations" -ForegroundColor Red
            exit 1
        }
        
        # Get SQL connection string from environment or prompt
        $SqlUser = Read-Host "SQL Server admin username"
        $SqlPassword = Read-Host "SQL Server admin password" -AsSecureString
        $SqlPasswordPlain = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
            [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($SqlPassword))
        
        $ConnectionString = "Server=tcp:$SqlServerName.database.windows.net,1433;Initial Catalog=$SqlDatabaseName;Persist Security Info=False;User ID=$SqlUser;Password=$SqlPasswordPlain;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
        
        Write-Host "Running migrations..." -ForegroundColor Yellow
        & "$PSScriptRoot\deploy-database.ps1" -ConnectionString $ConnectionString
        
        Write-Host "Migrations completed!" -ForegroundColor Green
        Write-Host ""
    }
    
    # Step 4: Get application URL
    Write-Host "Step 4: Getting application URL..." -ForegroundColor Cyan
    $containerAppInfo = az containerapp show `
        --name $ContainerAppName `
        --resource-group $ResourceGroup `
        --query "{fqdn: properties.configuration.ingress.fqdn, provisioningState: properties.provisioningState}" `
        -o json | ConvertFrom-Json
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Deployment Completed!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Application URL: https://$($containerAppInfo.fqdn)" -ForegroundColor Green
    Write-Host "Status: $($containerAppInfo.provisioningState)" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Verify API health: https://$($containerAppInfo.fqdn)/health" -ForegroundColor Gray
    Write-Host "2. Check logs: az containerapp logs show --name $ContainerAppName --resource-group $ResourceGroup --follow" -ForegroundColor Gray
    Write-Host "3. Update frontend NEXT_PUBLIC_API_URL to: https://$($containerAppInfo.fqdn)" -ForegroundColor Gray
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Deployment Failed!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    exit 1
} finally {
    Pop-Location
}
