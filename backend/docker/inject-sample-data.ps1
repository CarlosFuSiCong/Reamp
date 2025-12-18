#!/usr/bin/env pwsh
# Script to inject sample data into Docker SQL Server

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Reamp Sample Data Injection" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Check if Docker containers are running
Write-Host "Checking Docker containers..." -ForegroundColor Yellow
$containers = docker-compose ps --services --filter "status=running"

if ($containers -notcontains "sqlserver") {
    Write-Host "Error: SQL Server container is not running!" -ForegroundColor Red
    Write-Host "Please start containers first: docker-compose up -d" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ SQL Server container is running" -ForegroundColor Green
Write-Host ""

# Load environment variables
Write-Host "Loading environment variables..." -ForegroundColor Yellow
if (-not (Test-Path ".env")) {
    Write-Host "Error: .env file not found!" -ForegroundColor Red
    exit 1
}

$envVars = Get-Content ".env" | ForEach-Object {
    if ($_ -match '^([^=]+)=(.*)$') {
        $key = $matches[1]
        $value = $matches[2]
        [System.Environment]::SetEnvironmentVariable($key, $value, [System.EnvironmentVariableTarget]::Process)
    }
}

$dbPassword = $env:SQL_SERVER_PASSWORD
$dbName = $env:DB_NAME

if ([string]::IsNullOrEmpty($dbPassword) -or [string]::IsNullOrEmpty($dbName)) {
    Write-Host "Error: SQL_SERVER_PASSWORD or DB_NAME not set in .env file!" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Environment variables loaded" -ForegroundColor Green
Write-Host "  Database: $dbName" -ForegroundColor Gray
Write-Host ""

# Copy SQL script to container
Write-Host "Copying SQL script to container..." -ForegroundColor Yellow
$sqlScriptPath = "..\SampleData.sql"

if (-not (Test-Path $sqlScriptPath)) {
    Write-Host "Error: SampleData.sql not found at $sqlScriptPath" -ForegroundColor Red
    exit 1
}

docker cp $sqlScriptPath reamp-sqlserver:/tmp/SampleData.sql
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to copy SQL script to container" -ForegroundColor Red
    exit 1
}

Write-Host "✓ SQL script copied to container" -ForegroundColor Green
Write-Host ""

# Execute SQL script
Write-Host "Executing SQL script..." -ForegroundColor Yellow
Write-Host "This will create 8 sample listings..." -ForegroundColor Gray
Write-Host ""

$result = docker exec reamp-sqlserver /opt/mssql-tools18/bin/sqlcmd `
    -S localhost `
    -U sa `
    -P $dbPassword `
    -d $dbName `
    -i /tmp/SampleData.sql `
    -C

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "==================================" -ForegroundColor Green
    Write-Host "✓ Sample data injected successfully!" -ForegroundColor Green
    Write-Host "==================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Visit http://localhost:3000/listings to view the listings" -ForegroundColor White
    Write-Host "2. Upload images via Agent Dashboard to make listings more realistic" -ForegroundColor White
    Write-Host "3. See backend/SAMPLE-DATA-README.md for more details" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "Error: Failed to execute SQL script" -ForegroundColor Red
    Write-Host "Please check the error messages above" -ForegroundColor Yellow
    exit 1
}

# Clean up
Write-Host ""
Write-Host "Cleaning up temporary files..." -ForegroundColor Yellow
docker exec reamp-sqlserver rm /tmp/SampleData.sql
Write-Host "✓ Cleanup complete" -ForegroundColor Green
Write-Host ""
