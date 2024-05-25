# Short URL System

This project implements a URL shortening system with the following features:

- Shortens long URLs to unique short URLs
- Stores mappings in MongoDB, with no long URLs repeat
- Caches recently accessed URLs in memory
- Request collapsing for efficient multi-threading

**Technologies:**

- .NET Core (ASP)
- MongoDB
- Least Recently Used (LRU) caching strategy
- Request collapsing

**Installation:**

1. Clone the repository.
2. Install .NET Core SDK.
3. Restore NuGet packages: `dotnet restore`
4. Run the application: `dotnet run`

**Configuration:**

- Configure MongoDB connection details in `appsettings.json`:
