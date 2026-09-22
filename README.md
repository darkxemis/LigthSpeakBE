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

**LightSpeak** is the backend of a small, self-hosted communication platform inspired by Discord. The foundation is built on a **vertical-slice architecture**, where every feature owns its query/command, handler, validator and endpoint — easy to extend slice by slice.

The **authentication layer is fully implemented**: registration, login with JWT access and refresh tokens, HTTP-only cookies, BCrypt password hashing and account lockout.

## 🚀 Key Features

- 🔐 **Full authentication** — JWT access + refresh tokens, HTTP-only cookies, BCrypt hashing.
- 🛡️ **Login lockout** — blocks the account after repeated failed attempts.
- ✅ **Result pattern** — expected business failures returned as typed `Result<T>`, never as exceptions.
- 🚨 **Global exception safety net** — anything unexpected becomes a clean error response with `traceId`.
- 🧱 **Vertical slice + CQRS** — every use case is a self-contained slice under `Features/`, orchestrated by [MediatR](https://github.com/jbogard/MediatR).
- ✍️ **FluentValidation pipeline** — validators auto-run before every handler.
- 📦 **Auto-migrations + dev seed** — EF Core migrations applied on startup with retry logic.
- 📝 **Serilog** — console + rolling daily file sinks with request logging.
- 🐳 **Container-first** — one `docker compose up` brings API + SQL Server.
- 📄 **Modern API docs** — interactive [Scalar](https://scalar.com/) UI on top of native OpenAPI.

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

| Concern            | Implementation                                                   |
| ------------------ | ---------------------------------------------------------------- |
| API style          | ASP.NET Core **Minimal APIs** with endpoint groups               |
| Use cases          | **CQRS** (Queries & Commands) via MediatR pipeline behaviors     |
| Validation         | **FluentValidation** auto-wired through a `ValidationBehavior`   |
| Business errors    | **`Result<T>` + `Error`** (status, tag, message, metadata)       |
| Unexpected errors  | Global `IExceptionHandler` → clean error response + `traceId`    |
| Persistence        | **EF Core 10** + SQL Server, migrations applied on startup       |
| Auth               | JWT Bearer + refresh tokens, HTTP-only cookies, BCrypt, lockout  |
| Logging            | **Serilog** — console + daily rolling files + request logging    |
| API documentation  | **Scalar** UI on native `Microsoft.AspNetCore.OpenApi`           |

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
│       ├── Features/                     # ← vertical slices
│       │   └── Auth/
│       │       ├── Login/                #   LoginQuery + Handler + Validator
│       │       ├── Register/             #   RegisterCommand + Handler + Validator
│       │       ├── DTOs/                 #   AuthResponse
│       │       └── AuthEndpoints.cs      #   Auth endpoints
│       ├── Infrastructure/               # EF Core, JWT, BCrypt, middleware, DI
│       ├── Migrations/                   # EF Core migrations
│       ├── Program.cs                    # Application pipeline
│       └── Dockerfile
├── docker-compose.yml                    # API + SQL Server stack
├── .env                                  # Local config (git-ignored)
├── .gitignore
├── LICENSE
└── README.md
```

## 🚦 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Run with Docker

```bash
git clone https://github.com/darkxemis/LigthSpeakBE.git
cd LigthSpeakBE
docker compose up -d --build
```

### Run locally

```bash
docker compose up -d sqlserver          # database only
cd LightSpeak/LightSpeak.Backend
dotnet run
```

| Service              | URL                          |
| -------------------- | ---------------------------- |
| API (Docker)         | http://localhost:5000        |
| API (local)          | http://localhost:5133        |
| API docs (Scalar)    | `/scalar/v1`                 |

Interactive API documentation is available in Development via the Scalar UI.

## 🗺️ Roadmap

- 🔄 Refresh / logout endpoints
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
