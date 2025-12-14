# Reamp Docker Setup

## Prerequisites

- Docker Desktop installed
- .NET 8.0 SDK (for local development)
- Git

## Quick Start

### 1. Create Environment File

Copy `.env.example` to `.env` and update with your configuration:

```bash
cp .env.example .env
```

### 2. Start Services

```bash
docker-compose up -d
```

This will start:
- SQL Server database on `localhost:1433`
- ASP.NET Core API on `localhost:5000`

### 3. Verify Services

Check if services are running:

```bash
docker-compose ps
```

View logs:

```bash
docker-compose logs -f
```

## Docker Scripts

### Rebuild Everything

Use this when you've made code changes and need to rebuild the Docker image:

**Windows (PowerShell):**
```powershell
.\rebuild.ps1
```

**Linux/Mac:**
```bash
chmod +x rebuild.sh
./rebuild.sh
```

### Reset Database

Use this to completely reset the database (WARNING: deletes all data):

**Windows (PowerShell):**
```powershell
.\reset-database.ps1
```

**Linux/Mac:**
```bash
chmod +x reset-database.sh
./reset-database.sh
```

## Database Migrations

The application automatically applies pending migrations on startup.

To manually create a new migration:

```bash
cd ../src/Reamp
dotnet ef migrations add YourMigrationName --project Reamp.Infrastructure --startup-project Reamp.Api --context ApplicationDbContext
```

## Troubleshooting

### Database Connection Issues

1. Check if SQL Server container is healthy:
   ```bash
   docker-compose ps
   ```

2. Check SQL Server logs:
   ```bash
   docker-compose logs sqlserver
   ```

3. Verify connection string in `.env` file

### API Not Starting

1. Check API logs:
   ```bash
   docker-compose logs api
   ```

2. Verify all environment variables in `.env` file

3. Check if database is accessible:
   ```bash
   docker exec -it reamp-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword" -C
   ```

### Port Conflicts

If ports 5000 or 1433 are already in use, you can change them in `docker-compose.yml`:

```yaml
ports:
  - "5001:8080"  # Change 5000 to 5001
```

## Production Deployment

For production, use `docker-compose.prod.yml`:

```bash
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Useful Commands

```bash
# Stop all services
docker-compose down

# View logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f api
docker-compose logs -f sqlserver

# Restart a service
docker-compose restart api

# Execute commands in container
docker exec -it reamp-api bash

# Connect to SQL Server
docker exec -it reamp-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword" -C

# Remove all containers and volumes (DANGEROUS)
docker-compose down -v
```

## Role System Update (Dec 2024)

The role system has been updated:

### UserRole Changes
- `User` (1) → `Client` (1)
- `Client` (2) → `Agent` (2)

### AgencyRole Changes
- Removed: `Member` (0)
- Minimum role: `Agent` (1)

### StudioRole Changes
- Removed: `Member` (0), `Editor` (1), `Photographer` (2)
- New: `Staff` (1) (replaces all above)
- `Manager` (3) → `Manager` (2)
- `Owner` (4) → `Owner` (3)

The migration `20251214013949_UpdateRoleEnums` handles the data migration automatically.
