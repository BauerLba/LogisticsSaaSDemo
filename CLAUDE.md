# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**LogiFlow** is a SaaS logistics platform built with ASP.NET Core 8.0 Blazor (Interactive Server mode), PostgreSQL, and ASP.NET Core Identity. It follows Clean Architecture with three projects.

## Build & Run Commands

```bash
# Restore dependencies
dotnet restore

# Run the web app (development, falls back to in-memory DB if no connection string)
dotnet run --project LogisticsSaaS.Web
# Default URL: http://localhost:5140

# Build solution
dotnet build

# Add a new EF Core migration (run from repo root)
dotnet ef migrations add <MigrationName> --project LogisticsSaaS.Infrastructure --startup-project LogisticsSaaS.Web

# Apply migrations manually
dotnet ef database update --project LogisticsSaaS.Infrastructure --startup-project LogisticsSaaS.Web

# Docker (production)
docker-compose up --build
```

Migrations run automatically on startup via `dbContext.Database.Migrate()`.

## Architecture

**Three-layer Clean Architecture:**

- **LogisticsSaaS.Core** — Domain entities (`Shipment`, `Customer`, `ShipmentStatus`), repository interfaces (`IShipmentRepository`, `ICustomerRepository`), and application services (`ShipmentService`, `CustomerService`, `AppSettingsService`). No external dependencies.

- **LogisticsSaaS.Infrastructure** — EF Core implementations of repositories (`EfShipmentRepository`, `EfCustomerRepository`), `LogisticsDbContext` (extends `IdentityDbContext<ApplicationUser>`), and `ApplicationUser` identity model. Contains all migrations.

- **LogisticsSaaS.Web** — Blazor Server application. All UI lives under `Components/`. Pages use `@rendermode InteractiveServer`.

## Database

- **Production:** PostgreSQL via `DATABASE_URL` env var or `"DefaultConnection"` in `appsettings.json`.
- **Development fallback:** In-memory EF Core database when no connection string is set.
- **Seeding:** On startup, `SeedData()` creates sample customers and `SeedDemoUserAsync()` creates `demo@logiflow.com` / `Demo123!`.

## Authentication

Authentication uses form posts (not Blazor forms) to avoid Interactive Server SignalR limitations:
- `POST /do-login` — signs in and redirects
- `POST /do-register` — creates user and redirects
- `GET /logout` — signs out

All routes are protected via `AuthorizeRouteView` in [Routes.razor](LogisticsSaaS.Web/Components/Routes.razor). Unauthenticated users hit the `RedirectToLogin` component which navigates to `/login`. The `AuthLayout` is used for `/login` and `/register`; `MainLayout` for all other pages.

## Blazor Layout

```
Routes.razor (CascadingAuthenticationState + AuthorizeRouteView)
├── MainLayout.razor  — fixed sidebar (280px) + top header (80px)
└── AuthLayout.razor  — minimal layout for auth pages
```

The sidebar navigation and top header (search, notifications, user info, logout) live in [MainLayout.razor](LogisticsSaaS.Web/Components/Layout/MainLayout.razor).

## Styling

Glassmorphism dark theme using vanilla CSS. Key design tokens are CSS variables in `wwwroot/css/site.css`:
- `--primary: #6366f1` (indigo), `--accent: #22d3ee` (cyan), `--bg-dark: #0f172a`
- `.glass-card` for card surfaces with backdrop-filter blur
- `.status-badge` for shipment status chips

Icons: FontAwesome 6.4. Fonts: Inter (body), Outfit (headings).

## CI/CD

GitHub Actions workflow (`.github/workflows/deploy.yml`) triggers on push to `main`:
1. Builds a multi-arch Docker image (linux/arm64) and pushes to `ghcr.io/{owner}/logisticssaas:latest`
2. SSHes into Oracle Cloud VM and runs `docker compose up -d`

Required GitHub secrets: `ORACLE_HOST`, `ORACLE_SSH_KEY`.
