# PowerShell script to rebuild and restart Docker containers with database migration

Write-Host "=== Reamp Docker Rebuild Script ===" -ForegroundColor Cyan
Write-Host ""

# Stop and remove existing containers
Write-Host "Stopping and removing existing containers..." -ForegroundColor Yellow
docker-compose down

# Remove old images (optional - uncomment if needed)
# Write-Host "Removing old images..." -ForegroundColor Yellow
# docker rmi reamp-api

# Build new image
Write-Host "Building new image..." -ForegroundColor Yellow
docker-compose build --no-cache

# Start services
Write-Host "Starting services..." -ForegroundColor Yellow
docker-compose up -d

# Wait for services to be healthy
Write-Host "Waiting for services to be healthy..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check if database migration is needed
Write-Host "Checking database status..." -ForegroundColor Yellow
docker-compose logs api | Select-String -Pattern "migration"

Write-Host ""
Write-Host "=== Docker rebuild completed ===" -ForegroundColor Green
Write-Host "API is running at: http://localhost:5000" -ForegroundColor Green
Write-Host "SQL Server is running at: localhost:1433" -ForegroundColor Green
Write-Host ""
Write-Host "To view logs, run: docker-compose logs -f" -ForegroundColor Cyan
Write-Host "To stop services, run: docker-compose down" -ForegroundColor Cyan
