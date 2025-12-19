# Frontend Docker Build and Run Script

Write-Host "Building and starting Reamp Frontend..." -ForegroundColor Green

# Navigate to docker directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

# Check if .env file exists
if (-not (Test-Path ".env")) {
    Write-Host "Warning: .env file not found. Creating from env.example..." -ForegroundColor Yellow
    if (Test-Path "env.example") {
        Copy-Item "env.example" ".env"
        Write-Host "Please configure .env file before running again." -ForegroundColor Yellow
        exit 1
    }
}

# Stop and remove existing containers
Write-Host "Stopping existing containers..." -ForegroundColor Yellow
docker-compose down

# Rebuild images
Write-Host "Building Docker images..." -ForegroundColor Yellow
docker-compose build --no-cache

# Start services
Write-Host "Starting services..." -ForegroundColor Yellow
docker-compose up -d

# Show logs
Write-Host "`nFrontend is starting..." -ForegroundColor Green
Write-Host "View logs with: docker-compose logs -f" -ForegroundColor Cyan
Write-Host "Access frontend at: http://localhost:3000" -ForegroundColor Cyan

# Follow logs
docker-compose logs -f
