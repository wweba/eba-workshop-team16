# OrchardLite CMS - .NET Framework 4.8 Application

## Overview

This is a legacy ASP.NET Core 3.1 MVC application. It serves as the starting point for the AWS Modernization Workshop, demonstrating a typical enterprise application that needs modernization from .NET Core 3.1 to .NET 8.

## Technology Stack

- **Framework:** .NET Core 3.1 (End of Life: December 2022)
- **Web Framework:** ASP.NET Core MVC
- **Database:** MySQL 8.0 (via MySql.Data connector)
- **Configuration:** appsettings.json

## Legacy Patterns (for AWS Transform Detection)

This application uses .NET Core 3.1 which is now out of support and needs modernization:

1. **.NET Core 3.1** - End of life, security vulnerabilities (vs. .NET 8 LTS)
2. **Legacy MySQL connector** - MySql.Data (vs. modern Pomelo.EntityFrameworkCore.MySql)
3. **Startup.cs pattern** - Old configuration style (vs. minimal APIs in .NET 8)
4. **No dependency injection improvements** - Missing modern DI patterns
5. **Performance limitations** - Missing .NET 8 performance improvements

## Application Features

- **Content Management:** Display and manage blog posts and content items
- **Database Integration:** Connects to MySQL database for data storage
- **Health Check:** `/health` endpoint for monitoring
- **Bootstrap UI:** Responsive design matching workshop theme

## Environment Variables

The application expects the following environment variables:

- `DB_HOST` - MySQL database hostname
- `DB_PORT` - MySQL database port (default: 3306)
- `DB_NAME` - Database name (default: OrchardLiteDB)
- `DB_USER` - Database username
- `DB_PASSWORD` - Database password

## Building and Running

### Local Development

```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the application
cd OrchardLite.Web
dotnet run
```

### Docker Build

```bash
# Build Docker image
docker build -t orchardlite-cms:latest .

# Run container
docker run -p 80:80 \
  -e DB_HOST=your-db-host \
  -e DB_PORT=3306 \
  -e DB_NAME=OrchardLiteDB \
  -e DB_USER=your-user \
  -e DB_PASSWORD=your-password \
  orchardlite-cms:latest
```

## AWS Transform Modernization Path

After AWS Transform analyzes this application, it will recommend:

1. **Upgrade to .NET 8** - Latest LTS version with security updates
2. **Modernize Startup pattern** - Use minimal APIs and modern hosting
3. **Update MySQL connector** - Use Pomelo.EntityFrameworkCore.MySql
4. **Performance improvements** - Leverage .NET 8 performance enhancements
5. **Security updates** - Address .NET Core 3.1 EOL vulnerabilities
6. **Modern dependency injection** - Use latest DI patterns

## Workshop Flow

1. **Current State:** Deploy this .NET Core 3.1 application
2. **Analysis:** Use AWS Transform to analyze the codebase
3. **Transformation:** AWS Transform upgrades to .NET 8
4. **Deployment:** CodePipeline deploys the modernized version
5. **Validation:** Compare performance and features

## Database Schema

The application uses a simple `ContentItems` table:

```sql
CREATE TABLE ContentItems (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Title VARCHAR(255) NOT NULL,
  Summary TEXT,
  Body LONGTEXT,
  ContentType VARCHAR(50) NOT NULL DEFAULT 'BlogPost',
  AuthorId INT,
  PublishedDate DATETIME NOT NULL,
  ViewCount INT DEFAULT 0,
  IsPublished BOOLEAN DEFAULT TRUE,
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

## License

This is a workshop demonstration application. See main repository for license information.
