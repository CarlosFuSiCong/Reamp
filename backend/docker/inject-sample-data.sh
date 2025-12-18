#!/bin/bash
# Script to inject sample data into Docker SQL Server

echo "=================================="
echo "Reamp Sample Data Injection"
echo "=================================="
echo ""

# Check if Docker containers are running
echo "Checking Docker containers..."
if ! docker-compose ps | grep -q "reamp-sqlserver.*Up"; then
    echo "Error: SQL Server container is not running!"
    echo "Please start containers first: docker-compose up -d"
    exit 1
fi

echo "✓ SQL Server container is running"
echo ""

# Load environment variables
echo "Loading environment variables..."
if [ ! -f ".env" ]; then
    echo "Error: .env file not found!"
    exit 1
fi

export $(grep -v '^#' .env | xargs)

if [ -z "$SQL_SERVER_PASSWORD" ] || [ -z "$DB_NAME" ]; then
    echo "Error: SQL_SERVER_PASSWORD or DB_NAME not set in .env file!"
    exit 1
fi

echo "✓ Environment variables loaded"
echo "  Database: $DB_NAME"
echo ""

# Copy SQL script to container
echo "Copying SQL script to container..."
SQL_SCRIPT_PATH="../SampleData.sql"

if [ ! -f "$SQL_SCRIPT_PATH" ]; then
    echo "Error: SampleData.sql not found at $SQL_SCRIPT_PATH"
    exit 1
fi

docker cp "$SQL_SCRIPT_PATH" reamp-sqlserver:/tmp/SampleData.sql
if [ $? -ne 0 ]; then
    echo "Error: Failed to copy SQL script to container"
    exit 1
fi

echo "✓ SQL script copied to container"
echo ""

# Execute SQL script
echo "Executing SQL script..."
echo "This will create 8 sample listings..."
echo ""

docker exec reamp-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "$SQL_SERVER_PASSWORD" \
    -d "$DB_NAME" \
    -i /tmp/SampleData.sql \
    -C

if [ $? -eq 0 ]; then
    echo ""
    echo "=================================="
    echo "✓ Sample data injected successfully!"
    echo "=================================="
    echo ""
    echo "Next steps:"
    echo "1. Visit http://localhost:3000/listings to view the listings"
    echo "2. Upload images via Agent Dashboard to make listings more realistic"
    echo "3. See backend/SAMPLE-DATA-README.md for more details"
else
    echo ""
    echo "Error: Failed to execute SQL script"
    echo "Please check the error messages above"
    exit 1
fi

# Clean up
echo ""
echo "Cleaning up temporary files..."
docker exec reamp-sqlserver rm /tmp/SampleData.sql
echo "✓ Cleanup complete"
echo ""
