# Reamp - Real Estate Photography Marketplace

A full-stack platform connecting real estate agents with professional photography studios for property shoots, built with modern technologies and clean architecture principles.

![Next.js](https://img.shields.io/badge/Next.js-16.0-black)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED)

## 🌟 Features

### Core Functionality
- **Multi-Role System**: Admin, Agent, Staff, and User roles with granular permissions
- **Property Listings**: Create, manage, and showcase property listings with rich media
- **Order Management**: Complete photography order workflow from placement to delivery
- **Team Collaboration**: Agency and Studio team management with invitation system
- **Delivery System**: Secure media delivery with access control and expiration
- **Application System**: Professional application process for Agencies and Studios

### Technical Highlights
- **Domain-Driven Design (DDD)**: Clean architecture with separated concerns
- **CQRS Pattern**: Command/Query Responsibility Segregation for read/write operations
- **Real-time Updates**: SignalR for progress tracking and notifications
- **Media Management**: Cloudinary integration for asset storage
- **Responsive Design**: Mobile-first UI with modern components
- **Type Safety**: Full TypeScript and C# type coverage

## 🛠️ Tech Stack

### Frontend
- **Framework**: Next.js 16 (React 19) with App Router
- **Language**: TypeScript 5
- **Styling**: Tailwind CSS + shadcn/ui components
- **State Management**: TanStack Query (React Query) + Zustand
- **Forms**: React Hook Form + Zod validation
- **Real-time**: SignalR client

### Backend
- **Framework**: .NET 8.0
- **Architecture**: DDD with CQRS pattern
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Identity with JWT
- **Background Jobs**: Hangfire
- **Media Storage**: Cloudinary
- **API Documentation**: Swagger/OpenAPI

### Infrastructure
- **Containerization**: Docker + Docker Compose
- **Database**: SQL Server 2022 (Linux)
- **Package Manager**: pnpm (frontend), NuGet (backend)

## 🚀 Quick Start

### Prerequisites
- Node.js 20+ and pnpm
- .NET 8.0 SDK
- Docker Desktop
- SQL Server (or use Docker)

### 1. Clone the Repository
```bash
git clone <repository-url>
cd Reamp
```

### 2. Start Backend with Docker

```bash
cd backend/docker
docker-compose up -d
```

This will start:
- SQL Server on port 1433
- Backend API on port 5000

### 3. Start Frontend

```bash
cd frontend
pnpm install
pnpm dev
```

Frontend will be available at http://localhost:3000

### 4. Access the Application

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger

## 👤 Test Accounts

See [TEST-ACCOUNTS.md](./TEST-ACCOUNTS.md) for complete account information.

### Quick Access
| Role | Email | Password |
|------|-------|----------|
| Admin | admin@reamp.com | Test@123 |
| Agent | agent1@reamp.com | Test@123 |

## 📁 Project Structure

```
Reamp/
├── frontend/               # Next.js application
│   ├── src/
│   │   ├── app/           # App router pages
│   │   ├── components/    # React components
│   │   ├── lib/           # Utilities and hooks
│   │   └── types/         # TypeScript types
│   └── public/            # Static assets
│
├── backend/               # .NET solution
│   ├── src/Reamp/
│   │   ├── Reamp.Api/           # REST API layer
│   │   ├── Reamp.Application/   # Application services
│   │   ├── Reamp.Domain/        # Domain entities
│   │   └── Reamp.Infrastructure/ # Data access
│   └── docker/            # Docker configuration
│
└── .cursor/rules/         # Development rules
```

## 🔧 Development

### Frontend Development

```bash
cd frontend

# Install dependencies
pnpm install

# Start development server
pnpm dev

# Run type checking
pnpm type-check

# Run linting
pnpm lint

# Build for production
pnpm build
```

### Backend Development

```bash
cd backend/src/Reamp

# Restore packages
dotnet restore

# Run the API
dotnet run --project Reamp.Api

# Run tests
dotnet test

# Create migration
dotnet ef migrations add MigrationName --project Reamp.Infrastructure --startup-project Reamp.Api
```

### Database Management

```powershell
# Reset database (⚠️ Deletes all data)
cd backend/docker
.\reset-database.ps1

# Inject sample data
.\inject-sample-data.ps1
```

## 🏗️ Architecture

### Backend Architecture (DDD)

```
┌─────────────────────────────────────────┐
│          Reamp.Api (Presentation)       │
│  Controllers, Middleware, SignalR Hubs  │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│    Reamp.Application (Use Cases)        │
│  Commands, Queries, DTOs, Validators    │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      Reamp.Domain (Business Logic)      │
│  Entities, Value Objects, Repositories  │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│  Reamp.Infrastructure (Data & External) │
│  EF Core, Repositories, External APIs   │
└─────────────────────────────────────────┘
```

### Frontend Architecture

```
┌─────────────────────────────────────────┐
│         App Router (Pages)              │
│  Server/Client Components, Layouts      │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│       Components (UI Layer)             │
│  Reusable UI components, Forms          │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      Hooks & State (Data Layer)         │
│  TanStack Query, Zustand, API clients   │
└─────────────────────────────────────────┘
```

## 📋 Core Workflows

### 1. Agency Registration
```
User registers → Applies as Agency → Admin reviews → Approved → User becomes Agent
```

### 2. Order Creation
```
Agent creates Listing → Creates Order → Studio accepts → Assigns Staff → Completes shoot → Delivers media → Agent confirms
```

### 3. Team Management
```
Owner invites member → Member receives email → Accepts invitation → Joins team with assigned role
```

## 🔐 Environment Variables

### Frontend (.env.local)
```env
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_GOOGLE_MAPS_API_KEY=your_google_maps_key
```

### Backend (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ReampDb;..."
  },
  "Cloudinary": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  }
}
```

## 🧪 Testing

### Run All Tests
```bash
# Backend
cd backend/src/Reamp
dotnet test

# Frontend (when implemented)
cd frontend
pnpm test
```

## 📝 Code Style & Rules

This project follows strict coding conventions:
- **Commits**: Conventional Commits format
- **Frontend**: Next.js best practices (see `.cursor/rules/nextjs.mdc`)
- **Backend**: DDD patterns (see `.cursor/rules/ddd-backend.mdc`)
- **General**: Common rules (see `.cursor/rules/common.mdc`)

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feat/feature-name`
2. Follow commit conventions: `feat:`, `fix:`, `refactor:`, etc.
3. Ensure tests pass and code is formatted
4. Create a pull request

## 📄 License

This project is proprietary and confidential.

## 🔗 Links

- [Test Accounts](./TEST-ACCOUNTS.md) - Test user credentials and roles
- [API Documentation](http://localhost:5000/swagger) - Interactive API docs (when running)

## 🐛 Known Issues

- Google Maps integration requires API key configuration
- Some advanced features are still in development

## 📧 Support

For questions or issues, please contact the development team.

---

**Built with ❤️ using modern web technologies**
