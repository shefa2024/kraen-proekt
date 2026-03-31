# LearnConnect Deployment Guide

This application is designed to be deployed to modern cloud platforms like **Render**, **Heroku**, or **Azure**.

## 1. Prerequisites
- **Database**: The application requires an MS SQL Server. In production, use a managed instance (e.g., Azure SQL, Render PostgreSQL - though SQL Server is preferred by the project requirements). 
- **Environment Variables**: Configure the following secrets in your deployment platform:
  - `ConnectionStrings:DefaultConnection`: Production SQL Server connection string.
  - `Jwt:Secret`: A long, random string for token security.
  - `Stripe:SecretKey`: Your production Stripe secret key.

## 2. Deployment to Render (Recommended)
1. **Create a new Web Service**: Link your GitHub repository.
2. **Runtime**: Select `Docker` (if using Dockerfile) or `.NET`.
3. **Build Command**: `dotnet publish -c Release -o out`
4. **Start Command**: `dotnet LearnConnect.API.dll`
5. **Add Secrets**: Add the environment variables listed above.

## 3. Database Migrations
The application is configured to automatically apply migrations on startup (see `Program.cs`):
```csharp
context.Database.Migrate();
```
Ensure the production database user has enough permissions to create/update tables.

## 4. CI/CD
The repository includes a GitHub Actions workflow in `.github/workflows/ci-cd.yml` which automatically builds and tests the project on every push to the `main` branch.
