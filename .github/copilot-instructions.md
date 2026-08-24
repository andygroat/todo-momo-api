# copilot-instructions.md

## Overview
[todo-momo-api] - Modular Monolith Web API

## Tech Stack

- **.NET 10** / **ASP.NET Core** (Minimal APIs).
- **Entity Framework Core 10** (InMemory provider by default; SQL Server package available).
- **Serilog** / **Serilog.AspNetCore** for structured logging.
- **OpenAPI** via `AddOpenApi()` and **Scalar.AspNetCore** for the interactive UI.
- **MediatR 14** for commands, queries, and notifications.
- **FluentValidation 12** for validation.
- `Nullable` and `ImplicitUsings` are **enabled** in every project.

## Project Structure

`todo-momo-api` is an ASP.NET Core Web API built on **.NET 10**, organized as a modular monolith with a shared common library, feature modules, and per-project test suites.

| Project | Purpose |
| --- | --- |
| `todo-momo-api/` | ASP.NET Core Web API host (entry point, DI wiring, middleware, OpenAPI/Scalar, Serilog). |
| `todo-momo-api.common/` | Shared building blocks (MediatR, FluentValidation, logging abstractions). |
| `Modules.Todos/` | Todos feature module (EF Core, MediatR handlers, validators). |
| `todo-momo-api.tests/` | Tests for the web host. |
| `todo-momo-api.common.tests/` | Tests for the common library. |
| `Modules.Todos.Tests/` | Tests for the Todos module. |

## Code Conventions

- Target framework is `net10.0` — use modern C#/.NET 10 language and API features.
- Register application services via extension methods such as `builder.AddApplicationBuildingBlocks()` and endpoints via `app.MapWebApplication()`. Add new wiring as extension methods in `Extensions/` rather than inline in `Program.cs`.
- Keep `DbContext` and EF configuration inside the owning module (e.g. `Modules.Todos`).
- CQRS / mediation: One handler per request. Keep request/response DTOs alongside their handler.
- Validators registered via `FluentValidation.AspNetCore`. Place validators next to the requests they validate.
- **Modules:** New features go in a new `Modules.<Feature>` project that references `todo-momo-api.common`. Expose registration via a single `AddXxxModule` / `MapXxxModule` extension method consumed by the host.
- File-scoped namespaces.
- Primary constructors for dependency injection.
- Sealed classes for implementations.
- Internal by default, public only for contracts.
- Naming: PascalCase for types/members, camelCase for locals/parameters, `_camelCase` for private fields.
- Prefer `async`/`await` end-to-end; accept and propagate `CancellationToken`.
- Return `Results.*` / `TypedResults.*` from minimal API endpoints; avoid `IActionResult`.

## When Adding Code

1. Put feature code inthe appropriate `Modules.*` project; only cross-cutting abstractions belong in `todo-momo-api.common`.
2. Add or update a matching test in the corresponding `*.Tests` project.
3. Register new services / endpoints through existing extension methods.(`AddApplicationBuilingBlocks`, `MapWebApplication`) — do not mutate `Program.cs` directly.
4. Keep NuGet package versions aligned across projects (EF Core, MediatR, FluentValidation, Serilog).
5. Preserve nullable annotations and avoid `!` null-forgiving unless justified with a comment.

## Error Handling

- **Exception handling:** Rely on `app.UseExceptionHandler()` with the custom exception handlers already registered — add new handlers there instead of try/catching in endpoints.

## Testing

- Test Framework: **[TUnit](https://github.com/thomhurst/TUnit)** – use `[Test]`, `[Arguments(...)]`, and `await Assert.That(...)` (`TestingPlatformDotnetTestSupport` enabled).
- Code Coverage: `Microsoft.Testing.Extensions.CodeCoverage` produces Cobertura reports consumed by CI. Do not introduce xUnit/NUnit/MSTest.
- One test project per production project, mirroring namespace structure. New tests must use TUnit attributes (`[Test]`, `[Arguments]`, etc.).
- Use EF Core InMemory provider for handler/integration tests inside modules.

## Logging

- Use Serilog via injected `ILogger<T>`.
- Include contextual properties (e.g., `todoId`, `search`) using message templates, not string interpolation.

## Do

- ✅ Do target `.NET 10` idioms (primary constructors, collection expressions, `required` members where sensible).

## Don't

- ❌ Don't change target frameworks or downgrade package versions.
- ❌ Adding controllers-based MVC (project is minimal-API first).
- ❌ Introducing a different logger, mediator, validator, ORM, or test framework than those listed above.
- ❌ Placing business logic in `Program.cs` or in the host project instead of a module.
