# MistGo API - .NET Version

A REST API built with ASP.NET Core 9.0 for managing user authentication and items.

## Features

- User registration and login with JWT authentication
- CRUD operations for items (Create, Read, Update, Delete)
- PostgreSQL database with Entity Framework Core
- Docker containerization for easy deployment
- CORS enabled for frontend integration

## Prerequisites

- .NET 9.0 SDK (for local development)
- Docker & Docker Compose (for containerized deployment)
- PostgreSQL (if running without Docker)

## Running with Docker (Recommended)

1. Navigate to the project directory:
```bash
cd /Users/federico/Projects/EyeGo/mistgo-api-dotnet
```

2. Start the services:
```bash
docker-compose up -d
```

This will start:
- API on http://localhost:5001
- PostgreSQL database on localhost:5432

3. Test the API:
```bash
# Health check
curl http://localhost:5001/health

# Register a new user
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"password123"}'
```

4. Stop the services:
```bash
docker-compose down
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token

### Items (Requires Authentication)
- `GET /api/items` - Get all items for the authenticated user
- `GET /api/items/{id}` - Get a specific item
- `POST /api/items` - Create a new item
- `PUT /api/items/{id}` - Update an item
- `DELETE /api/items/{id}` - Delete an item

### Health
- `GET /health` - Health check endpoint

## Running Locally (Without Docker)

1. Update the connection string in `MistGoApi/appsettings.json` to point to your PostgreSQL instance.

2. Navigate to the MistGoApi folder:
```bash
cd MistGoApi
```

3. Restore dependencies:
```bash
dotnet restore
```

4. Run the application:
```bash
dotnet run
```

The API will be available at http://localhost:5000 (or the port specified in launchSettings.json)

## Project Structure

```
mistgo-api-dotnet/
├── MistGoApi/
│   ├── Controllers/      # API Controllers
│   ├── Data/             # Database Context
│   ├── DTOs/             # Data Transfer Objects
│   ├── Models/           # Entity Models
│   ├── Services/         # Business Logic Services
│   ├── Program.cs        # Application Entry Point
│   ├── appsettings.json  # Configuration
│   └── Dockerfile        # Container Definition
└── docker-compose.yml    # Multi-container Setup
```

## Frontend Integration

To integrate with your frontend:
1. Update the API URL in your frontend's `api.js` file to `http://localhost:5001`
2. The API has CORS enabled to accept requests from any origin during development

## Security Notes

- The JWT key in appsettings.json is for development only
- Use environment variables or Azure Key Vault for production secrets
- Update CORS policy for production to only allow your frontend domain