<!-- Improved compatibility of back to top link -->
<a id="readme-top"></a>

# todo-momo-api

A modular, minimal-API-based Todo REST service built on **.NET 10** and ASP.NET Core. The project demonstrates a clean, modular monolith architecture using the vertical slice / feature-folder pattern powered by MediatR, FluentValidation, and Entity Framework Core.

`todo-momo-api` exposes a small HTTP API for managing to-do items (create, list, get by id, and mark complete). It is designed as a reference for organizing an ASP.NET Core application into cleanly separated modules, each owning its own domain, features, and infrastructure.

## Table of Contents

- [Solution Structure](#solution-structure)
- [Features](#features)
- [Architecture](#architecture)
- [Built With](#built-with)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Build & Run](#build--run)
  - [Database](#database)

## Solution Structure

The solution (`todo-momo-api.slnx`) is composed of three projects:

| Project | Purpose |
| --- | --- |
| `todo-momo-api` | ASP.NET Core host application. Wires up dependency injection, OpenAPI and Serilog-based structred logging. |
| `Modules.Todos` | The Todos feature module. Contains the **Domain** entities such as `TodoItem` (inheriting from a shared `BusinessObject` base). **Features** which are vertical slices for each use case (`CreateTodo`, `GetTodos`, `GetTodoById`, `CompleteTodo`) using MediatR commands/queries, FluentValidation validators, handlers, and minimal-API endpoint mappings. EF Core `TodoDbContext` and schema definitions. Module registration entry point (`AddTodoModule`, `MapTodoEndpoints`). |
| `todo-momo-api.common` | Shared building blocks used across modules. Behaviours — MediatR pipeline behaviors: `LoggingBehavior` and `ValidationBehavior`. ResultHelper — `Result<T>` and `Error` types for functional-style error handling. |

## Features

The API currently supports the following todo operations:

| Method | Route                        | Description                                          |
| ------ | ---------------------------- | ---------------------------------------------------- |
| POST   | `/api/todo`                  | Create a new todo item.                              |
| GET    | `/api/todo?search=...`       | List todo items, optionally filtered by description. |
| GET    | `/api/todo/{id}`             | Retrieve a single todo item by id.                   |
| POST   | `/api/todo/{id}/complete`    | Mark a todo item as completed.                       |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Architecture

- **Modular monolith**: Each feature area lives in its own module project and registers itself through a single `AddXxxModule` / `MapXxxEndpoints` pair.
- **Vertical slices**: Every feature file (e.g. `CreateTodo.cs`) contains its command/query, validator, handler, and endpoint mapping side-by-side.
- **Cross-cutting behaviors**: Logging and validation are applied globally through MediatR `IPipelineBehavior` implementations in `todo-momo-api.common`.
- **Result pattern**: Handlers return `Result<T>` instead of throwing, providing predictable success/error flows.
- **In-memory database by default**: `TodoModule` configures EF Core with an in-memory store (`"TodoDb"`) for easy local development; a SQL Server registration is included as a commented example.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Built With

| Logo | Technology | Purpose |
| :---: | --- | --- |
| ![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white) | **.NET 10 / ASP.NET Core** | Minimal APIs host and runtime |
| ![EF Core](https://img.shields.io/badge/EF_Core_10-512BD4?style=for-the-badge&logo=nuget&logoColor=white) | **Entity Framework Core 10** | Data access (in-memory for dev; SQL Server package included) |
| ![MediatR](https://img.shields.io/badge/MediatR_14-BA0C2F?style=for-the-badge&logo=mediatek&logoColor=white) | **MediatR 14** | Request/response pipeline for commands and queries |
| ![FluentValidation](https://img.shields.io/badge/FluentValidation_11-2C8EBB?style=for-the-badge&logo=checkmarx&logoColor=white) | **FluentValidation 11** | Declarative request validation |
| ![Serilog](https://img.shields.io/badge/Serilog-4B8BBE?style=for-the-badge&logo=serilog&logoColor=white) | **Serilog** | Structured logging to console |
| ![OpenAPI](https://img.shields.io/badge/OpenAPI-6BA539?style=for-the-badge&logo=openapiinitiative&logoColor=white) | **OpenAPI** | API description via `AddOpenApi()` |
| [![Scalar](https://img.shields.io/badge/Scalar-1B1F23?style=for-the-badge&logo=scalar&logoColor=white)](https://scalar.com/) | **Scalar** | Interactive API document UI |

## Getting Started

### Prerequisites:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) (Community edition or higher) with the **ASP.NET and web development** workload.

### Build & Run

1. Clone the repository:

   ```powershell
   git clone https://github.com/andygroat/todo-momo-api.git
   cd todo-momo-api
   ```
2. Open `todo-momo-api/todo-momo-api.slnx` in Visual Studio 2026.
3. Restore NuGet packages (Visual Studio does this automatically on load, or run `dotnet restore`).
4. Configure the database connection string in `todo-momo-api/todo-momo-api/appsettings.json` (used by `TodoDbContext`).
5. Set `todo-momo-api` as the startup project and run.

The API starts on the URLs listed in `todo-momo-Api/Properties/launchSettings.json`. OpenAPI and Scalar UI are enabled for exploring the endpoints.

### Database

By default the API registers `TodoDbContext` with EF Core's **in-memory** provider (`TodoDb`) for zero-setup local development. To switch to a real provider (e.g., SQL Server), replace the registration in `Infrastructure/Extensions/WebApplicationBuilderExtensions.AddDatabaseContext` and supply a connection string via `appsettings.json` -> `ConnectionStrings`.

<p align="right">(<a href="#readme-top">back to top</a>)</p>
