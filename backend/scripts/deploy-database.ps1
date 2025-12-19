# Azure SQL Database Deployment Script
# This script runs EF Core migrations against Azure SQL Database

param(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString,
    
    [Parameter(Mandatory=$false)]
    [switch]$WithSeedData
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Azure SQL Database Migration" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to API project directory
$ApiProjectPath = Join-Path $PSScriptRoot ".." "src" "Reamp" "Reamp.Api"
Push-Location $ApiProjectPath

try {
    Write-Host "Checking EF Core tools..." -ForegroundColor Yellow
    dotnet tool restore
    
    Write-Host ""
    Write-Host "Running database migrations..." -ForegroundColor Green
    Write-Host "Connection: $($ConnectionString.Substring(0, 50))..." -ForegroundColor Gray
    Write-Host ""
    
    # Run migrations
    dotnet ef database update --connection "$ConnectionString" --verbose
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Migration completed successfully!" -ForegroundColor Green
        
        if ($WithSeedData) {
            Write-Host ""
            Write-Host "Seeding test data..." -ForegroundColor Yellow
            # TODO: Add seed data script execution here
            Write-Host "Note: Please use test scripts to seed data manually" -ForegroundColor Gray
        }
    } else {
        Write-Host "Migration failed!" -ForegroundColor Red
        exit 1
    }
} finally {
    Pop-Location
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database is ready!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

