#!/bin/bash

# Frontend Docker Build and Run Script

echo "Building and starting Reamp Frontend..."

# Navigate to docker directory
cd "$(dirname "$0")"

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo "Warning: .env file not found. Creating from env.example..."
    if [ -f "env.example" ]; then
        cp env.example .env
        echo "Please configure .env file before running again."
        exit 1
    fi
fi

# Stop and remove existing containers
echo "Stopping existing containers..."
docker-compose down

# Rebuild images
echo "Building Docker images..."
docker-compose build --no-cache

# Start services
echo "Starting services..."
docker-compose up -d

# Show logs
echo ""
echo "Frontend is starting..."
echo "View logs with: docker-compose logs -f"
echo "Access frontend at: http://localhost:3000"

# Follow logs
docker-compose logs -f
