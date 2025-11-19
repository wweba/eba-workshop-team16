# OrchardLite CMS

A legacy .NET Core 3.1 content management system built with ASP.NET Core MVC and MySQL.

## Overview

OrchardLite CMS is a sample enterprise content management application that demonstrates common patterns found in legacy .NET applications. The application provides basic content management capabilities including creating, viewing, and organizing blog posts and articles.

## Technology Stack

- **.NET Core 3.1** (End of Life: December 2022)
- **ASP.NET Core MVC** - Web framework
- **MySQL 8.0** - Database
- **Bootstrap 5** - UI framework
- **Docker** - Containerization

## Features

- Content item management (blog posts, articles, pages)
- MySQL database integration
- Responsive Bootstrap UI
- Health check endpoint
- Automatic database initialization with sample data
- Docker support for containerized deployment

## Application Structure

```
/OrchardLiteApp/
├── OrchardLite.sln              # Solution file
├── Dockerfile                    # Container definition
└── OrchardLite.Web/             # MVC application
    ├── Controllers/             # MVC controllers
    ├── Models/                  # Data models
    ├── Views/                   # Razor views
    ├── Services/                # Business logic
    └── appsettings.json         # Configuration
```

## Database Schema

The application uses a simple content management schema:

**ContentItems Table:**
- Id (Primary Key)
- Title
- Summary
- Body
- ContentType (BlogPost, Article, Page, Tutorial)
- AuthorId
- PublishedDate
- ViewCount
- IsPublished
- CreatedDate

## Environment Variables

The application requires the following environment variables:

- `DB_HOST` - MySQL database hostname
- `DB_PORT` - MySQL database port (default: 3306)
- `DB_NAME` - Database name (default: OrchardLiteDB)
- `DB_USER` - Database username
- `DB_PASSWORD` - Database password

## Running Locally

### Prerequisites
- .NET Core 3.1 SDK
- MySQL 8.0
- Docker (optional)

### Using .NET CLI

```bash
# Navigate to the application directory
cd OrchardLiteApp/OrchardLite.Web

# Set environment variables
export DB_HOST=localhost
export DB_PORT=3306
export DB_NAME=OrchardLiteDB
export DB_USER=root
export DB_PASSWORD=yourpassword

# Run the application
dotnet run
```

### Using Docker

```bash
# Build the Docker image
cd OrchardLiteApp
docker build -t orchardlite-cms:latest .

# Run the container
docker run -p 80:80 \
  -e DB_HOST=your-db-host \
  -e DB_PORT=3306 \
  -e DB_NAME=OrchardLiteDB \
  -e DB_USER=your-user \
  -e DB_PASSWORD=your-password \
  orchardlite-cms:latest
```

## Endpoints

- `/` - Home page with content listing
- `/all-content` - View all content items
- `/health` - Health check endpoint (returns JSON)

## Database Initialization

The application automatically initializes the database on first run:
- Creates the `OrchardLiteDB` database if it doesn't exist
- Creates the `ContentItems` table
- Seeds 100 sample content records for demonstration

## Legacy Patterns

This application intentionally uses legacy patterns typical of older .NET applications:

- .NET Core 3.1 (end of life, no longer supported)
- Legacy MySQL connector (MySql.Data)
- Traditional Startup.cs configuration pattern
- No modern performance optimizations
- Missing security best practices

## License

This is a demonstration application for educational purposes.
