#!/bin/bash

# Bash script to reset database (delete and recreate)

echo "=== Reamp Database Reset Script ==="
echo ""
echo "WARNING: This will DELETE all data in the database!"
read -p "Are you sure you want to continue? (yes/no): " confirmation

if [ "$confirmation" != "yes" ]; then
    echo "Operation cancelled."
    exit 1
fi

echo ""
echo "Stopping containers..."
docker-compose down

echo "Removing database volume..."
docker volume rm reamp_sqlserver_data 2>/dev/null || true

echo "Starting services..."
docker-compose up -d

echo ""
echo "=== Database reset completed ==="
echo "The database will be recreated with all migrations applied."
echo ""
echo "To view logs, run: docker-compose logs -f api"
