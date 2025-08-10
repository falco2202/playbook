# PlayBook Codebase Overview

PlayBook is a .NET 8 solution organized along clean architecture lines. Projects are split into layers:

| Layer/Project | Purpose |
| --- | --- |
| Domain | Core business types; e.g. `ApplicationUser` entity and configuration helpers. |
| Application | Contracts and domain-event infrastructure that describe what the app can do without referencing external tech. |
| Infrastructure | Implementations for authentication, persistence, and cross-cutting services (EF Core, ASP.NET Identity, JWT handling, cookie utilities, etc.). |
| Presentation | ASP.NET Core Web API exposing endpoints (e.g., authentication and a sample weather forecast). |
| playbook.ServiceDefaults & playbook.AppHost | Shared defaults and hosting scaffolding for distributed apps using .NET Aspire features. |
| SharedKernel | Placeholder for reusable code shared across services (currently empty). |
| Presentation.Tests & tests/PlayBook.Tests | xUnit test projects (currently mostly stubs). |

## Important Concepts

1. **Layered Dependency Flow**
   - `Presentation` depends on `Application` and `Infrastructure`.
   - `Infrastructure` depends on `Application`.
   - `Application` depends on `Domain`.
   This keeps business logic independent from framework details.

2. **Domain Events & Aggregate Roots**
   The `Application` layer defines `AggregateRoot` and event interfaces so domain events can be collected and dispatched after `UnitOfWork.SaveChangesAsync` executes.

3. **Authentication & Identity**
   The `Infrastructure.Identity` namespace implements JWT-based auth with ASP.NET Identity. Token generation helpers live in a partial class (`AuthenticateService` + `AuthenticateExtension`). Refresh- and revoke-token methods are stubbed for future work.

4. **Persistence**
   `PlayBookDbContext` derives from `IdentityDbContext` and registers custom tables (e.g., `RefreshTokens`). `PersistenceExtensions` wires up EF Core with SQL Server, while `UnitOfWork` coordinates saving changes and dispatching domain events.

5. **Presentation Layer**
   `Program.cs` registers services, configures Swagger, and sets up middleware. Controllers handle REST endpoints (e.g., `AuthenticateController` manages login and token refresh, while `WeatherForecastController` is a sample template).

6. **Service Defaults & Hosting**
   `playbook.ServiceDefaults` adds common concerns such as OpenTelemetry, health checks, service discovery, and resilience handlers. `playbook.AppHost` uses `DistributedApplication.CreateBuilder` to host the API in a distributed environment.

7. **Testing Infrastructure**
   Two test projects are present but mostly empty. `Presentation.Tests` references all major projects for integration-style tests; `tests/PlayBook.Tests` is a generic xUnit project targeting .NET 9.

## Pointers for Further Exploration

- **MediatR & Domain Events** – Understand how requests/notifications are handled and how domain events propagate through `DomainEventDispatcher`.
- **ASP.NET Identity & JWT** – Study how `AuthenticateService` interacts with `UserManager`/`SignInManager`, generates tokens, and stores refresh tokens.
- **Entity Framework Core** – Inspect `PlayBookDbContext`, seeding logic, and how repositories/unit-of-work manage data access.
- **OpenTelemetry & Service Discovery** – Explore `playbook.ServiceDefaults` to learn about distributed tracing and health checks in .NET Aspire.
- **Testing Strategy** – Flesh out the stubbed test projects with unit and integration tests, leveraging xUnit and Moq.
- **Security Enhancements** – Implement the missing refresh-token and revoke-token flows, and review Identity configuration for production readiness.
