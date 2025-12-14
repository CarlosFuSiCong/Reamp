# PowerShell script to reset database (delete and recreate)

Write-Host "=== Reamp Database Reset Script ===" -ForegroundColor Red
Write-Host ""
Write-Host "WARNING: This will DELETE all data in the database!" -ForegroundColor Red
$confirmation = Read-Host "Are you sure you want to continue? (yes/no)"

if ($confirmation -ne "yes") {
    Write-Host "Operation cancelled." -ForegroundColor Yellow
    exit
}

Write-Host ""
Write-Host "Stopping containers..." -ForegroundColor Yellow
docker-compose down

Write-Host "Removing database volume..." -ForegroundColor Yellow
docker volume rm reamp_sqlserver_data -ErrorAction SilentlyContinue

Write-Host "Starting services..." -ForegroundColor Yellow
docker-compose up -d

Write-Host ""
Write-Host "=== Database reset completed ===" -ForegroundColor Green
Write-Host "The database will be recreated with all migrations applied." -ForegroundColor Green
Write-Host ""
Write-Host "To view logs, run: docker-compose logs -f api" -ForegroundColor Cyan
