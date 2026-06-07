# Rentify Backend

Rentify Backend is the API foundation for a multi-tenant SaaS platform focused on rental car businesses. The project is designed to support tenant onboarding, subscription management, user administration, rental car registration, and secure authentication workflows.

This is my first SaaS project and part of my professional portfolio. The goal is to demonstrate how I structure a real backend product with clean architecture principles, modular application features, validation, authentication, persistence, and production-minded API design.

## Project Overview

Rentify is being built as a backend service for rental car companies that need a centralized system to manage their business data. Each company can be represented as a tenant, with its own users, subscription information, and rental car records.

The backend currently focuses on the core platform foundation:

- Tenant registration and onboarding
- User creation and role assignment
- Authentication with JWT access tokens
- Refresh token persistence and token rotation
- Forgot password and reset password flows
- Rental car registration
- Subscription domain modeling
- FluentValidation-based request validation
- Entity Framework Core persistence with PostgreSQL

## Architecture

The solution follows a layered architecture that separates business rules, application use cases, infrastructure concerns, and API presentation.

### Core.Domain

Contains the domain entities, value objects, enums, and settings used by the application. This layer represents the business concepts of Rentify, such as tenants, subscriptions, rental cars, and shared domain values.

### Core.Application

Contains the application modules, commands, handlers, validators, service contracts, shared responses, and pipeline behaviors. MediatR is used to organize feature flows, and FluentValidation is used to validate incoming requests before handlers execute.

### Infraestructure.Persistence

Contains the main application database context, entity configurations, repositories, migrations, and persistence registration for core business data.

### Infraestructure.Identity

Contains ASP.NET Core Identity integration, JWT generation, authentication services, refresh token persistence, identity database context, identity migrations, default roles, and default users.

### Presentation.WebApi

Contains the ASP.NET Core Web API entry point, middleware registration, endpoint mapping, Swagger configuration, health checks, authentication, and authorization setup.

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core
- PostgreSQL
- MediatR
- FluentValidation
- JWT Bearer Authentication
- Swagger / OpenAPI

## Authentication Features

The authentication module includes:

- Login with username and password
- JWT access token generation
- Refresh token creation and storage
- Refresh token rotation
- Refresh token revocation
- Forgot password token generation
- Password reset

Refresh tokens are stored in the Identity database and are linked to application users. When a refresh token is used, it is revoked and replaced with a new refresh token, reducing the risk of token reuse.

## API Modules

Current API modules include:

- Auth
- Users
- Tenants
- Rent Cars
- Subscriptions

Endpoints are organized as minimal API route mappings and grouped by feature.

## Validation

Requests are validated with FluentValidation. The application uses a MediatR validation pipeline behavior, so command validators run before the command handlers. This keeps validation rules close to each feature while keeping handlers focused on business logic.

## Database

The project uses Entity Framework Core with PostgreSQL. It currently contains separate database contexts for:

- Application persistence data
- Identity and authentication data

Migrations are included for both persistence and identity infrastructure.

## Portfolio Goals

This project is intended to show:

- Backend API development with .NET
- SaaS-oriented architecture
- Multi-tenant domain modeling
- Authentication and authorization implementation
- Clean separation between layers
- Use of validation, repositories, services, and migrations
- Practical experience building a real product foundation

## Status

Rentify Backend is under active development. The current version focuses on the platform foundation and core backend workflows. Future work may include richer tenant administration, subscription billing integration, reporting, availability management, reservations, and frontend integration.

## Running the Project

Configure the required database and JWT settings, then run the Web API project:

```bash
dotnet run --project Rentify.Backend/Rentify.Backend.Presentation.WebApi/Rentify.Backend.Presentation.WebApi.csproj
```

The API documentation is available through Swagger when running in development mode.
