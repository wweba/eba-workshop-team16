# Multi-stage build for .NET Core 3.1 application
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /app

# Copy solution and project files
COPY OrchardLite.sln ./
COPY OrchardLite.Web/OrchardLite.Web.csproj ./OrchardLite.Web/

# Restore NuGet packages
RUN dotnet restore

# Copy everything else
COPY . ./

# Build and publish the application
RUN dotnet publish OrchardLite.Web/OrchardLite.Web.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app

# Copy published application
COPY --from=build /app/publish ./

# Expose port 80
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "OrchardLite.Web.dll"]
