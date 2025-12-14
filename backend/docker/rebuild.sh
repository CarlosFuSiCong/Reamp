#!/bin/bash

# Bash script to rebuild and restart Docker containers with database migration

echo "=== Reamp Docker Rebuild Script ==="
echo ""

# Stop and remove existing containers
echo "Stopping and removing existing containers..."
docker-compose down

# Remove old images (optional - uncomment if needed)
# echo "Removing old images..."
# docker rmi reamp-api

# Build new image
echo "Building new image..."
docker-compose build --no-cache

# Start services
echo "Starting services..."
docker-compose up -d

# Wait for services to be healthy
echo "Waiting for services to be healthy..."
sleep 10

# Check if database migration is needed
echo "Checking database status..."
docker-compose logs api | grep -i "migration"

echo ""
echo "=== Docker rebuild completed ==="
echo "API is running at: http://localhost:5000"
echo "SQL Server is running at: localhost:1433"
echo ""
echo "To view logs, run: docker-compose logs -f"
echo "To stop services, run: docker-compose down"
