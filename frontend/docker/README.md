# Reamp Frontend - Docker Setup

Docker configuration for the Reamp Next.js frontend application.

## Prerequisites

- Docker Desktop installed
- Docker Compose V2
- Backend API running (or configured URL)

## Quick Start

### Windows (PowerShell)

```powershell
cd frontend/docker
.\rebuild.ps1
```

### Linux/Mac

```bash
cd frontend/docker
chmod +x rebuild.sh
./rebuild.sh
```

## Configuration

### Environment Variables

Copy `env.example` to `.env` and configure:

```bash
# Backend API URL
NEXT_PUBLIC_API_URL=http://localhost:5000

# Google Maps API Key (optional)
NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=your_api_key_here

# Resend Email Service (for demo requests)
RESEND_API_KEY=your_resend_api_key_here
DEMO_REQUEST_RECIPIENT_EMAIL=your_email@example.com
```

## Docker Commands

### Build and Run

```bash
# Development mode (with hot reload)
docker-compose up

# Production mode
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up

# Build without cache
docker-compose build --no-cache
```

### Stop Services

```bash
docker-compose down

# Remove volumes
docker-compose down -v
```

### View Logs

```bash
# All logs
docker-compose logs -f

# Specific service
docker-compose logs -f frontend
```

## Docker Image

The Dockerfile uses a multi-stage build:

1. **Dependencies Stage**: Installs Node.js dependencies using pnpm
2. **Builder Stage**: Builds the Next.js application
3. **Runner Stage**: Minimal production image with only necessary files

### Image Size Optimization

- Uses `node:20-alpine` for smaller image size
- Multi-stage build removes build dependencies
- Standalone output mode for minimal runtime
- Non-root user for security

## Networking

The frontend connects to the backend via the `reamp-network` Docker network.

### Connect to Backend

Ensure the backend is running and accessible:

```bash
# Create shared network if not exists
docker network create reamp-network

# Start backend first
cd ../../backend/docker
docker-compose up -d

# Then start frontend
cd ../../frontend/docker
docker-compose up -d
```

## Production Deployment

### VPS Deployment

Deploy to your VPS using docker-compose:

```bash
# 1. Copy files to VPS
scp -r frontend/docker user@your-vps:/opt/reamp/frontend/

# 2. SSH into VPS
ssh user@your-vps

# 3. Navigate to directory
cd /opt/reamp/frontend/docker

# 4. Create .env file with production settings
cat > .env << EOF
NEXT_PUBLIC_API_URL=https://api.yourdomain.com
NEXT_PUBLIC_APP_URL=https://yourdomain.com
NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=your_key_here
RESEND_API_KEY=your_key_here
DEMO_REQUEST_RECIPIENT_EMAIL=your_email@example.com
EOF

# 5. Deploy with production config
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Docker Registry (Optional)

If you want to use a private registry:

```bash
# Build and tag
docker build -t registry.yourdomain.com/reamp-frontend:latest -f docker/Dockerfile .

# Push to registry
docker push registry.yourdomain.com/reamp-frontend:latest

# Pull and run on VPS
docker pull registry.yourdomain.com/reamp-frontend:latest
docker-compose up -d
```

## Troubleshooting

### Port Already in Use

```bash
# Find process using port 3000
netstat -ano | findstr :3000  # Windows
lsof -i :3000                  # Linux/Mac

# Stop the process or change port in docker-compose.yml
```

### Build Failures

```bash
# Clean Docker cache
docker system prune -a

# Rebuild without cache
docker-compose build --no-cache
```

### Hot Reload Not Working

Development mode with hot reload requires volume mounting (configured in `docker-compose.override.yml`).

## Performance

### Resource Limits

Production configuration (`docker-compose.prod.yml`) sets:
- CPU: 0.5-1 core
- Memory: 256MB-512MB

Adjust based on your traffic:

```yaml
resources:
  limits:
    cpus: '2'
    memory: 1G
```

### Scaling

```bash
# Scale to 3 replicas
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --scale frontend=3
```

## Security

- Non-root user (nextjs:nodejs)
- Minimal base image (Alpine Linux)
- No dev dependencies in production
- Environment variables for secrets
- Read-only file system (can be enabled)

## Monitoring

### Health Check

Add to `docker-compose.yml`:

```yaml
healthcheck:
  test: ["CMD", "wget", "--quiet", "--tries=1", "--spider", "http://localhost:3000/api/health"]
  interval: 30s
  timeout: 10s
  retries: 3
  start_period: 40s
```

### Logs

```bash
# Export logs
docker-compose logs > logs.txt

# Follow specific container
docker logs -f <container-id>
```

## References

- [Next.js Docker Documentation](https://nextjs.org/docs/deployment#docker-image)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)
- [Docker Production Best Practices](https://docs.docker.com/develop/dev-best-practices/)
