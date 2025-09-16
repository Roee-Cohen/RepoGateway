# 📡 RepoGateway

RepoGateway is the **entrypoint API service** in the Repository Platform.  
It connects clients to repository metadata, favorites, and analysis reports, while coordinating with downstream services such as **RepoAnalyzer**.  
It also integrates with **Redis** for caching and **rate limiting**.

---

## ✨ Features

- 👤 **User Management**
  - Fetch user by ID (from first file)
  - Create runtime-only users
- ⭐ **Favorites**
  - Store repositories as favorites
  - Attach analysis reports to favorites
- 📊 **Analysis Integration**
  - Connects to **RepoAnalyzer** for enriched insights
- 🗄️ **Redis**
  - Response caching for frequently accessed data
  - Stores rate-limit counters per user/IP
- 🚦 **Rate Limiting**
  - Protects the API from abuse
  - Limits number of requests per time window
- 🔒 **Secure Configuration**
  - `secrets.json` via .NET User Secrets (no secrets in Git)
- 📝 **Logging & Error Handling**
  - Structured logs (console + debug)
  - Centralized exception handling middleware

---

## 🏗 Architecture

```
flowchart LR
  Client --> RepoGateway
  RepoGateway --> RepoAnalyzer
  RepoGateway --> Postgres[(Postgres DB)]
  RepoGateway --> RabbitMQ[(RabbitMQ Bus)]
  RepoGateway --> Redis[(Redis Cache)]
```

## AppSettings
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "Postgres": ""
  },
  "RabbitMQ": {
    "Host": "",
    "User": "",
    "Pass": ""
  },
  "Redis": {
    "Host": "localhost",
    "Port": 6379
  },
  "RateLimit": {
    "PermitLimit": 100,
    "WindowSeconds": 60
  }
}
```

## Secrets
```
cd src/RepoGateway
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Database=repo;Username=dev;Password=secret"
dotnet user-secrets set "RabbitMQ:Host" "localhost"
dotnet user-secrets set "RabbitMQ:User" "guest"
dotnet user-secrets set "RabbitMQ:Pass" "guest"
dotnet user-secrets set "Redis:Host" "localhost"
```

## 🚦 Rate Limiting

- Default: 100 requests per 60 seconds per user/IP
- Counters stored in Redis
- Exceeding the limit returns:
```
{
  "error": "Rate limit exceeded. Try again later."
}
```

## 📑 API Endpoints
Base URL: http://localhost:5000/api
🔹 Favorites
```
Get all favorites
GET /api/favorites
Authorization: Bearer <token>
```

Response 200 OK
```
[
  {
    "id": 1,
    "repoId": "662182",
    "name": "HebrewLexicon",
    "owner": "openscriptures",
    "stars": 131,
    "updatedAt": "2025-08-20T00:19:43Z",
    "userId": "a4d6b752-b338-484b-9d21-faac78bd8808",
    "analysisState": "completed",
    "analysis": {
      "id": "060f563e-0dde-470a-99b9-08830d1",
      "repoId": "662182",
      "owner": "openscriptures",
      "name": "HebrewLexicon",
      "licenseSpdxId": "MIT",
      "topics": ["bible", "hebrew", "lexicon"],
      "primaryLanguage": "C#",
      "readmeLength": 1500,
      "openIssues": 12,
      "forks": 5,
      "starsSnapshot": 131,
      "activityDays": 300,
      "defaultBranch": "main",
      "healthScore": 0.85,
      "analyzedAt": "2025-08-20T00:19:43Z"
    }
  }
]
```
Add a favorite
```
POST /api/favorites
Authorization: Bearer <token>
Content-Type: application/json
```
Response 202 Accepted

Remove a favorite
```
DELETE /api/favorites/{repoId}
Authorization: Bearer <token>
```
Response 204 No Content

🔹 Repository Search
Search repositories
```
GET /api/repo?q=lexicon
Authorization: Bearer <token>
```
Response 200 OK
```
{
  "items": [
    {
      "repoId": "662182",
      "name": "HebrewLexicon",
      "owner": "openscriptures",
      "stars": 131,
      "htmlUrl": "https://github.com/openscriptures/HebrewLexicon",
      "updatedAt": "2025-08-20T00:19:43Z"
    }
  ]
}
```

🔹 Health
Service health check
```
GET /api/healthcheck
```
Response 200 OK
```
{
  "status": "Healthy",
  "checks": [
    { "component": "Postgres", "status": "Healthy", "description": "Connected" },
    { "component": "RabbitMQ", "status": "Healthy", "description": "Connected" },
    { "component": "Redis", "status": "Healthy", "description": "Connected" }
  ],
  "duration": 45.7
}
```

## 🧪 Testing

Unit tests use xUnit + Moq.

Create a test project:
```
dotnet new xunit -n RepoGateway.Tests
dotnet add RepoGateway.Tests reference src/RepoGateway
dotnet add RepoGateway.Tests package Moq
dotnet sln add tests/RepoGateway.Tests/RepoGateway.Tests.csproj
```

Run tests:
```
dotnet test

🚀 Run Locally
# Restore dependencies
dotnet restore

# Run API
dotnet run --project src/RepoGateway
```

Dependencies (Postgres, RabbitMQ, Redis) can be run with Docker:
```
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=secret postgres:15
docker run -d -p 5672:5672 rabbitmq:3-management
docker run -d -p 6379:6379 redis:7
```
## 🧩 Related Services

- RepoAnalyzer → Performs deep repository analysis
- Client → UI for users to search and favorite repositories

## 📌 Roadmap

- Add Swagger/OpenAPI docs

- Add Docker Compose for full stack

- Integration tests for Redis + RabbitMQ

- Extend analysis health scoring

## 📜 License

MIT
