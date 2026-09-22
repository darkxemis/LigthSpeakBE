<div align="center">

# 💬 LightSpeak

**A lightweight, Discord-inspired communication platform — built with vertical-slice architecture.**

Backend for real-time chat: auth today, servers and messages tomorrow.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-14-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![MediatR](https://img.shields.io/badge/MediatR-CQRS-8B5CF6)](https://github.com/jbogard/MediatR)
[![Serilog](https://img.shields.io/badge/Serilog-Structured-00B3C7?logo=serilog&logoColor=white)](https://serilog.net/)
[![Scalar](https://img.shields.io/badge/Scalar-API_Docs-2B7A9E?logo=scalar&logoColor=white)](https://scalar.com/)

</div>

---

## ✨ Overview

**LightSpeak** is the backend of a small, self-hosted communication platform inspired by Discord. The foundation is already in place: a **vertical-slice architecture** where every feature owns its query/command, handler, validator and endpoint — easy to extend slice by slice.

The **authentication layer is fully implemented**: registration, login with JWT access tokens, refresh tokens, HTTP-only cookies, BCrypt hashing and account lockout — following the same battle-tested patterns as [Budgexa](https://github.com/darkxemis/Budgexa).

## 🚀 Key Features

- 🔐 **Full authentication** — `register` + `login`, JWT access token (15 min) + refresh token (7 days), HTTP-only cookies with `X-Skip-Cookies` support for API clients.
- 🧂 **BCrypt password hashing** — with configurable password policy (length, upper/lower, digit, special).
- 🛡️ **Login lockout** — blocks the account after repeated failed attempts (configurable attempts + minutes).
- ✅ **Result pattern** — expected business failures (`invalidCredentials`, `emailAlreadyExists`, validation errors…) returned as typed `Result<T>` + `Error`, never as exceptions.
- 🚨 **Global exception safety net** — `IExceptionHandler` converts anything unexpected into a clean error response with `traceId` (stack trace only in Development).
- 🧱 **Vertical slice + CQRS** — every use case is a self-contained slice under `Features/`, orchestrated by [MediatR](https://github.com/jbogard/MediatR) pipeline behaviors.
- ✍️ **FluentValidation pipeline** — validators auto-run before every handler and reply with field-level errors.
- 📦 **Auto-migrations + dev seed** — EF Core migrations run on startup with retry logic; a dev admin user is seeded on first boot.
- 📝 **Serilog** — console + rolling daily file sinks (`logs/`), request logging middleware.
- 🐳 **Container-first** — one `docker compose up` brings API + SQL Server, all configuration injected from `.env`.
- 📦 **Modern API docs** — interactive [Scalar](https://scalar.com/) UI on top of native OpenAPI.

## 🏗️ Architecture

The backend follows the **vertical slice** pattern inside a single project: cross-cutting concerns live in `Common/`, each feature is an isolated slice in `Features/`, and technical details (EF Core, JWT, hashing) live in `Infrastructure/`.

```
┌──────────────────────────────────────────────────────────┐
│  LightSpeak.Backend                                      │
│   ├─ Common/          ← entities, interfaces, Result,    │
│   │                     ValidationBehavior, ErrorTags    │
│   ├─ Features/Auth/   ← login & register slices          │
│   │   ├─ Login/       (Query + Handler + Validator)      │
│   │   ├─ Register/    (Command + Handler + Validator)    │
│   │   └─ AuthEndpoints (Minimal API mapping)             │
│   └─ Infrastructure/  ← EF Core, JWT, BCrypt, Serilog    │
│                         middleware, DI wiring            │
└──────────────────────────────────────────────────────────┘
```

| Concern            | Implementation                                                       |
| ------------------ | -------------------------------------------------------------------- |
| API style          | ASP.NET Core **Minimal APIs** with endpoint groups                   |
| Use cases          | **CQRS** (Queries & Commands) via MediatR pipeline behaviors         |
| Validation         | **FluentValidation** auto-wired through a `ValidationBehavior`       |
| Business errors    | **`Result<T>` + `Error`** (status code, tag, message, metadata)      |
| Unexpected errors  | Global `IExceptionHandler` → `ApiErrorResponse` + `traceId`          |
| Persistence        | **EF Core 10** + SQL Server, migrations applied on startup           |
| Auth               | JWT Bearer + refresh tokens, HTTP-only cookies, BCrypt, lockout      |
| Logging            | **Serilog** — console + daily rolling files + request logging        |
| API documentation  | **Scalar** UI on native `Microsoft.AspNetCore.OpenApi`               |

## 🧰 Tech Stack

- **.NET 10** (C# 14, nullable + implicit usings enabled)
- ASP.NET Core Minimal APIs
- Entity Framework Core 10 (SQL Server 2022)
- MediatR 14 · FluentValidation 12
- BCrypt.Net-Next · Serilog · Scalar
- Docker / Docker Compose

## 📁 Repository Structure

```
LigthSpeakBE/
├── LightSpeak/
│   ├── LightSpeak.slnx                   # Solution
│   └── LightSpeak.Backend/
│       ├── Common/                       # Entities, interfaces, Result, behaviors
│       │   ├── Behaviors/                #   ValidationBehavior (MediatR pipeline)
│       │   ├── Constants/                #   RoleNames
│       │   ├── Entities/                 #   Entity, User, Role, UserRole, RefreshToken
│       │   ├── Exceptions/               #   ErrorTags
│       │   ├── Interfaces/               #   IApplicationDbContext, IPasswordHasher, …
│       │   └── Results/                  #   Result<T> + Error
│       ├── Features/                     # ← vertical slices
│       │   └── Auth/
│       │       ├── Login/                #   LoginQuery + Handler + Validator
│       │       ├── Register/             #   RegisterCommand + Handler + Validator
│       │       ├── DTOs/                 #   AuthResponse
│       │       └── AuthEndpoints.cs      #   POST /login, /register
│       ├── Infrastructure/
│       │   ├── Authentication/           #   JwtTokenGenerator, BCrypt, settings
│       │   ├── Middleware/               #   GlobalExceptionHandler, ApiErrorResponse
│       │   ├── Persistence/              #   DbContext, configurations, DbSeeder
│       │   └── DependencyInjection.cs    #   EF Core + JWT wiring
│       ├── Migrations/                   # EF Core migrations
│       ├── Program.cs                    # Pipeline: Serilog → CORS → Auth → slices
│       ├── DependencyInjection.cs        # Presentation + Application (MediatR)
│       ├── Dockerfile
│       └── appsettings.json              # Non-secret defaults only
├── docker-compose.yml                    # API + SQL Server stack
├── .env                                  # ⚠️ Secrets & config (git-ignored)
└── .gitignore
```

## 🚦 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Option A — Full stack with Docker

```bash
git clone <your-repo-url>
cd LigthSpeakBE
docker compose up -d --build
```

### Option B — Local development (API on your machine)

1. Start only the database:

   ```bash
   docker compose up -d sqlserver
   ```

2. Run the API (connection + JWT secret come from *user-secrets*):

   ```bash
   cd LightSpeak/LightSpeak.Backend
   dotnet run
   ```

| Service    | URL                                    |
| ---------- | -------------------------------------- |
| API (Docker) | http://localhost:5000                |
| API (local, http) | http://localhost:5133           |
| API docs (Scalar) | `http://localhost:<port>/scalar/v1` |

> On first boot the API applies pending migrations and seeds a dev account (Development only).

### 🔑 Dev seed account

| Field    | Value                 |
| -------- | --------------------- |
| Email    | `admin@lightspeak.dev` |
| Password | `Admin123!`            |
| Roles    | `admin`, `member`      |

## 🔌 API

Base path: `/api/v1/auth` — interactive docs at `/scalar/v1`.

### Register *(open endpoint — development)*

```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "email": "user@lightspeak.dev",
  "password": "Pass123!secret",
  "firstName": "John",
  "lastName": "Doe"
}
```

**201 Created** → `{ "userId": "…" }` · **400** validation · **409** `user.emailAlreadyExists`

### Login

```http
POST /api/v1/auth/login
Content-Type: application/json
X-Skip-Cookies: true        # API/mobile clients (web clients omit it → cookies set)

{
  "email": "admin@lightspeak.dev",
  "password": "Admin123!"
}
```

**200 OK**

```json
{
  "userId": "f4adc9f5-…",
  "email": "admin@lightspeak.dev",
  "fullName": "Admin LightSpeak",
  "accessToken": "eyJhbGciOiJIUzI1NiIs…",
  "refreshToken": "Rx1sfjJcFBKdB9/…"
}
```

| Status | Tag                        | When                          |
| ------ | -------------------------- | ----------------------------- |
| 400    | `validation.failed`        | Missing/malformed fields      |
| 401    | `auth.invalidCredentials`  | Unknown email or wrong password |
| 403    | `auth.accountLocked`       | Too many failed attempts      |

### Error envelope

Every error — from `Result` failures or the global handler — uses the same shape:

```json
{
  "tag": "auth.invalidCredentials",
  "message": "Invalid email or password.",
  "traceId": "0H…",
  "metadata": null,
  "detail": null
}
```

## ⚙️ Configuration

Precedence: **`.env` / environment variables** → *user-secrets* → `appsettings.json`.

| Section / variable            | Purpose                                     |
| ----------------------------- | ------------------------------------------- |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string        |
| `JwtSettings__Secret`         | HMAC-SHA256 signing secret (≥ 64 bytes)     |
| `JwtSettings__Issuer/Audience`| Token issuer & audience                     |
| `JwtSettings__ExpirationInMinutes` | Access token lifetime (default 15)      |
| `JwtSettings__RefreshTokenExpirationInDays` | Refresh token lifetime (default 7) |
| `LoginLockout__MaxFailedAttempts` / `LockoutMinutes` | Lockout policy |
| `Cors__AllowedOrigins__0`     | Allowed frontend origins                    |
| `Serilog__MinimumLevel__*`    | Log levels (default / AspNetCore / EF)      |
| `Logging__LogLevel__*`        | built-in logging levels                     |
| `AllowedHosts`                | Host filtering                              |

All of these are wired in `docker-compose.yml` and their values live in `.env`.

> 🔒 **Secrets never touch git.** `appsettings.json` ships placeholders only; real values come from *user-secrets* (local) or `.env` (Docker). The `.gitignore` excludes `.env`, `bin/`, `obj/`, logs and user files — verified with `git check-ignore`.

## 🗺️ Roadmap

- 🔄 `refresh` / `logout` endpoints (token rotation already modelled in `RefreshToken`)
- 👤 Profile & user management
- 🏘️ Servers (guilds), channels & memberships
- 💬 Messages & real-time delivery (WebSockets)
- 🧪 Test suite

## 📜 License

This project is **proprietary software**. All rights reserved by the copyright holder. See [LICENSE](./LICENSE) for details.

No part of this software may be reproduced, distributed, modified, or used in any form without prior written permission.

---

<div align="center">

Made with ❤️, ☕ and a lot of <code>dotnet watch</code>.

</div>
