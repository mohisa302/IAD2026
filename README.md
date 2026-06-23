# IAD2026 – Enterprise API Management Template

## Overview

This project is a modular **.NET 8 enterprise API management template** designed for large-scale systems with:

- Multiple external API integrations (OAuth2, session-based, API key)
- Multiple databases (SQL Server, PostgreSQL, etc.)
- Background job processing
- Centralized caching and logging
- Clean Architecture principles

The goal is to provide a **scalable, maintainable, and cross-cutting architecture foundation** for enterprise systems such as telecom platforms (e.g., Irancell).

---

# Architecture

## High-Level Structure

```text
IAD2026.Api
IAD2026.Application
IAD2026.Domain
IAD2026.Shared

IAD2026.Infrastructure
IAD2026.Persistence
IAD2026.Integrations
IAD2026.Caching
IAD2026.Logging
IAD2026.BackgroundJobs

IAD2026.Tests
```

---

## Layer Responsibilities

### Domain

- Core business entities
- Enums
- Value Objects
- Domain rules
- No external dependencies

---

### Application

- Use cases (CQRS via MediatR)
- DTOs
- Validation (FluentValidation)
- Abstractions (interfaces for external services)

Depends on:

- Domain
- Shared

---

### Shared

- Common primitives:

  - `Result<T>`
  - `Error`
  - `PagedResult<T>`

- Shared constants and extensions

No dependencies

---

### Persistence

- Entity Framework Core DbContexts
- Repository implementations
- Database configurations

Supports multiple databases:

- SQL Server
- PostgreSQL
- InMemory (testing)

---

### Integrations

- External API clients
- OAuth2 / API Key / Session-based authentication
- HTTP communication (HttpClient + Polly)

Examples:

- SharePoint API
- CRM API
- Billing systems
- SAP integrations

---

### Caching

- In-memory caching
- Redis caching (future-ready)
- Abstraction via `ICacheService`

Used for:

- API tokens
- Frequently accessed data
- External API response caching

---

### Logging

- Serilog-based structured logging
- File + Console sinks
- Enriched logs (correlation IDs, environment, etc.)

Abstracted via:

- `IAppLogger`

---

### BackgroundJobs

- Hangfire-based job processing
- Recurring jobs
- Delayed execution jobs

Examples:

- Data sync jobs
- Token refresh jobs
- Cleanup jobs

---

### Infrastructure

- Composition root
- Wires all modules together
- Contains no business logic

---

### API

- ASP.NET Core Web API
- Controllers
- Middlewares
- Swagger
- Authentication (future)

---

# Key Design Principles

## 1. Cross-Cutting Abstractions

All system-wide concerns are abstracted:

- Logging → `IAppLogger`
- Caching → `ICacheService`
- External APIs → `IExternalClient`
- Persistence → `IRepository`

---

## 2. Separation of Concerns

Each module owns:

- Its implementation
- Its dependencies
- Its registration logic

via:

```csharp
services.AddPersistence(configuration);
services.AddCaching();
services.AddIntegrations(configuration);
services.AddLoggingModule(configuration);
services.AddBackgroundJobs();
```

---

## 3. Dependency Flow

```text
Api
 └── Infrastructure
      ├── Persistence
      ├── Integrations
      ├── Caching
      ├── Logging
      └── BackgroundJobs

Application → Domain + Shared
Infrastructure → Modules
```

---

## 4. External API Strategy

All external APIs are isolated behind:

- Typed clients
- Credential providers (OAuth2 / API Key / Session)
- Central HTTP executor with resilience (Polly)

This ensures:

- No duplicated authentication logic
- No duplicated retry logic
- Centralized observability

---

## 5. Database Strategy

Supports multiple DbContexts:

- Each domain area can have its own context
- Repositories are grouped per bounded context
- No shared DbContext across unrelated domains

---

## 6. Background Processing Strategy

Uses Hangfire for:

- Persistent jobs
- Retry support
- Dashboard monitoring

---

# Tech Stack

- .NET 8
- ASP.NET Core Web API
- MediatR
- FluentValidation
- Entity Framework Core
- Serilog
- Hangfire
- Polly
- StackExchange.Redis
- Mapster

---

# Current Status

This template is in early-stage setup:

✔ Project structure created
✔ Clean architecture defined
✔ Cross-cutting modules separated
⏳ Dependency implementations pending
⏳ External API layer design pending
⏳ Database contexts pending

---

# Future Improvements

- OAuth2 credential provider system
- Central API gateway layer
- Distributed caching (Redis cluster)
- Observability stack (OpenTelemetry)
- Multi-database transaction strategy
- Modular plugin-based integrations

---
