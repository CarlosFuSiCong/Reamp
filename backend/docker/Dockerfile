# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/Reamp/Reamp.sln", "src/Reamp/"]
COPY ["src/Reamp/Reamp.Api/Reamp.Api.csproj", "src/Reamp/Reamp.Api/"]
COPY ["src/Reamp/Reamp.Application/Reamp.Application.csproj", "src/Reamp/Reamp.Application/"]
COPY ["src/Reamp/Reamp.Application.Read/Reamp.Application.Read.csproj", "src/Reamp/Reamp.Application.Read/"]
COPY ["src/Reamp/Reamp.Domain/Reamp.Domain.csproj", "src/Reamp/Reamp.Domain/"]
COPY ["src/Reamp/Reamp.Infrastructure/Reamp.Infrastructure.csproj", "src/Reamp/Reamp.Infrastructure/"]
COPY ["src/Reamp/Reamp.Infrastructure.Read.EF/Reamp.Infrastructure.Read.EF.csproj", "src/Reamp/Reamp.Infrastructure.Read.EF/"]
COPY ["src/Reamp/Reamp.Shared/Reamp.Shared.csproj", "src/Reamp/Reamp.Shared/"]

# Restore dependencies
RUN dotnet restore "src/Reamp/Reamp.Api/Reamp.Api.csproj"

# Copy the rest of the code
COPY src/Reamp/ src/Reamp/

# Build the application
WORKDIR "/src/src/Reamp/Reamp.Api"
RUN dotnet build "Reamp.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Reamp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "Reamp.Api.dll"]

