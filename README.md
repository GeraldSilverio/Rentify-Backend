# Rentify Backend

Rentify is a multi-tenant SaaS backend for car rental companies. The project is designed as a portfolio-grade backend platform focused on tenant onboarding, subscription management, authentication, company profiles, vehicle inventory, customer management, reservations, payments, invoicing, email templates, and dashboard metrics.

The solution follows Clean Architecture principles with a modular application layer, domain entities that protect business invariants, CQRS-style commands and queries with MediatR, FluentValidation, Entity Framework Core, PostgreSQL, JWT authentication, and external service integrations isolated behind application abstractions.

## Main Capabilities

- Multi-tenant onboarding with owner account creation.
- JWT authentication, refresh tokens, forgot password, and reset password flows.
- Subscription plans, trial logic, expiration handling, and inactive subscription validation.
- Tenant-aware rent car company profiles with Cloudinary logo uploads.
- Normalized vehicle catalog with brands, models, vehicle types, vehicles, images, status, and availability blocks.
- Customer CRUD with duplicate license validation per tenant and document uploads.
- Reservation creation with date overlap validation, vehicle availability checks, rental day calculation, and total amount calculation.
- Payment registration with reservation payment status updates and invoice generation.
- Dashboard metrics filtered by tenant: monthly revenue, active reservations, available vehicles, and upcoming returns.
- Email template management with template codes and tenant email configuration support.

## Architecture

The backend is organized around the following layers:

- `Core.Domain`: business entities, value objects, enums, and domain rules.
- `Core.Application`: commands, queries, handlers, validators, DTOs, service contracts, and application services.
- `Infraestructure.Persistence`: EF Core DbContext, entity configurations, repositories, Unit of Work, and migrations.
- `Infraestructure.Identity`: ASP.NET Core Identity, authentication, authorization, JWT, and refresh token persistence.
- `Shared`: external service implementations such as email and file storage providers.
- `Presentation.WebApi`: API composition root, dependency registration, middleware, Swagger, and endpoint mapping.

The application layer is organized by business modules:

- `Tenants`
- `Security`
- `Subscriptions`
- `RentCars`
- `Vehicles`
- `Customers`
- `Reservations`
- `Payments`
- `Dashboard`
- `Emails`

## Technology Stack

- .NET 8
- ASP.NET Core Minimal APIs
- MediatR
- FluentValidation
- Entity Framework Core
- PostgreSQL with Npgsql
- ASP.NET Core Identity
- JWT Bearer Authentication
- Cloudinary for image storage
- Resend-ready email provider abstraction
- Swagger / OpenAPI

## Environment Configuration

Rentify reads sensitive configuration from environment variables. For local development, create a `.env` file in the API startup path or repository root.

Example:

```env
DB_HOST=localhost
DB_PORT=5432
DB_DATABASE_NAME=rentify
DB_USER=postgres
DB_PASSWORD=your_password

JWT_SECRET_KEY=your_super_secret_key
JWT_ISSUER=Rentify
JWT_AUDIENCE=RentifyUsers
JWT_EXPIRATION_MINUTES=60

CLOUDINARY_CLOUD_NAME=your_cloud_name
CLOUDINARY_API_KEY=your_api_key
CLOUDINARY_API_SECRET=your_api_secret

RESEND_API_KEY=your_resend_api_key
EMAIL_FROM=noreply@rentify.app
EMAIL_FROM_NAME=Rentify
```

Do not commit `.env` files or provider credentials.

## Running The Project

Restore dependencies:

```powershell
dotnet restore
```

Build the solution:

```powershell
dotnet build Rentify.Backend.slnx
```

Run the API:

```powershell
dotnet run --project Rentify.Backend/Rentify.Backend.Presentation.WebApi/Rentify.Backend.Presentation.WebApi.csproj
```

Swagger is available in development mode at:

```text
https://localhost:{port}/swagger
```

## Database Migrations

Create a migration:

```powershell
dotnet ef migrations add MigrationName `
  --project Rentify.Backend/Rentify.Backend.Infraestructure.Persistence/Rentify.Backend.Infraestructure.Persistence.csproj `
  --startup-project Rentify.Backend/Rentify.Backend.Presentation.WebApi/Rentify.Backend.Presentation.WebApi.csproj `
  --context RentifyContext
```

Apply migrations:

```powershell
dotnet ef database update `
  --project Rentify.Backend/Rentify.Backend.Infraestructure.Persistence/Rentify.Backend.Infraestructure.Persistence.csproj `
  --startup-project Rentify.Backend/Rentify.Backend.Presentation.WebApi/Rentify.Backend.Presentation.WebApi.csproj `
  --context RentifyContext
```

## API Areas

Most business endpoints are protected with JWT Bearer authentication. Public endpoints include authentication and tenant onboarding flows.

Key secured endpoint groups include:

- `/api/v1/rent-cars`
- `/api/v1/vehicle-brands`
- `/api/v1/tenants/{tenantId}/vehicles`
- `/api/v1/tenants/{tenantId}/customers`
- `/api/v1/tenants/{tenantId}/reservations`
- `/api/v1/tenants/{tenantId}/payments`
- `/api/v1/tenants/{tenantId}/dashboard/metrics`
- `/api/v1/emails`
- `/api/v1/subscriptions`

## Design Notes

- All tenant-owned business data includes `TenantId`.
- Cross-tenant access is prevented in repository queries and application services.
- Endpoints only map HTTP requests and delegate work to MediatR.
- Business rules live in domain entities and application services.
- FluentValidation handles request validation before handlers execute.
- Repositories encapsulate persistence queries.
- Unit of Work coordinates database commits.
- External providers are accessed through abstractions to keep infrastructure replaceable.

## Portfolio Context

Rentify is being built as a real SaaS foundation for the car rental industry and as a professional portfolio project. The codebase emphasizes maintainability, modularity, clean boundaries, and production-oriented patterns that can evolve into a complete rental operations platform.
