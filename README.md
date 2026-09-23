<div align="center">

# 💬 LightSpeak

**A lightweight, Discord-inspired communication platform — built with vertical-slice architecture.**

Real-time backend: auth, servers, channels, messages, text chat and voice signaling.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-14-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![SignalR](https://img.shields.io/badge/SignalR-Realtime-0078D4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core/signalr/)
[![MediatR](https://img.shields.io/badge/MediatR-CQRS-8B5CF6)](https://github.com/jbogard/MediatR)
[![Serilog](https://img.shields.io/badge/Serilog-Structured-00B3C7?logo=serilog&logoColor=white)](https://serilog.net/)
[![Scalar](https://img.shields.io/badge/Scalar-API_Docs-2B7A9E?logo=scalar&logoColor=white)](https://scalar.com/)

</div>

---

## ✨ Overview

**LightSpeak** is the backend of a small, self-hosted communication platform inspired by Discord. It uses a **vertical-slice architecture**: every feature owns its query/command, handler, validator and endpoint — easy to extend slice by slice.

Today the app covers:

- **Auth & profile** — register, login, refresh/logout, profile image
- **Servers (guilds)** — create, join by invite code, rename, leave, delete, icons
- **Members & roles** — list members, kick, promote/demote (`Member` / `Admin` / `Owner`)
- **Channels** — text & voice channels with ordered positions
- **Messages** — send, edit, delete, cursor-based history pagination, rate limit
- **Real-time** — SignalR chat hub + WebRTC voice signaling hub

## 🚀 Key Features

- 🔐 **Full authentication** — JWT access + refresh tokens, HTTP-only cookies, BCrypt hashing; mobile clients can opt out of cookies (`X-Skip-Cookies`).
- 🛡️ **Login lockout** — blocks the account after repeated failed attempts.
- 🏘️ **Servers** — create with default `#general` text + voice channels; join via invite code; regenerate invite; leave or delete (owner only).
- 🎨 **Server icons** — upload/replace/delete image icons (admin+), served as static files.
- 👥 **Members & roles** — improved permissions over a plain member list:
  - **Owner** — delete server, change roles, kick anyone (except themselves)
  - **Admin** — rename server, manage channels/icons/invites, kick members
  - **Member** — read/write messages, leave the server
- 💬 **Messages** — send / edit (author) / delete (author or admin+), history with cursor pagination, simple rate limit.
- 📡 **ChatHub** (`/hubs/chat`) — join/leave channel groups, live messages, typing indicators.
- 🎙️ **VoiceHub** (`/hubs/voice`) — WebRTC signaling only (offers/answers/ICE); media travels P2P between clients.
- ✅ **Result pattern** — expected business failures returned as typed `Result<T>`, never as exceptions.
- 🚨 **Global exception safety net** — anything unexpected becomes a clean error response with `traceId`.
- 🧱 **Vertical slice + CQRS** — every use case is a self-contained slice under `Features/`, orchestrated by [MediatR](https://github.com/jbogard/MediatR).
- ✍️ **FluentValidation pipeline** — validators auto-run before every handler.
- 📦 **Auto-migrations + dev seed** — EF Core migrations applied on startup with retry logic.
- 📝 **Serilog** — console + rolling daily file sinks with request logging.
- 🐳 **Container-first** — one `docker compose up` brings API + SQL Server.
- 📄 **Modern API docs** — interactive [Scalar](https://scalar.com/) UI on top of native OpenAPI.

## 🏗️ Architecture

The backend follows the **vertical slice** pattern inside a single project: domain types live in `Dominio/`, cross-cutting concerns in `Common/`, each feature is an isolated slice in `Features/`, and technical details (EF Core, JWT, SignalR wiring) live in `Infrastructure/`.

```
┌──────────────────────────────────────────────────────────────┐
│  LightSpeak.Backend                                          │
│   ├─ Common/            ← Result, interfaces, ErrorTags,     │
│   │                        ValidationBehavior                │
│   ├─ Dominio/           ← entities, enums, Entity base       │
│   ├─ Features/          ← vertical slices                    │
│   │   ├─ Auth/          (login, register, refresh, logout)   │
│   │   ├─ Users/         (profile, profile image)             │
│   │   ├─ Servers/       (CRUD, invite, members, icons)       │
│   │   ├─ Channels/      (list, create, delete)               │
│   │   ├─ Messages/      (history, send, edit, delete)        │
│   │   ├─ Chat/          (ChatHub)                            │
│   │   └─ Voice/         (VoiceHub signaling)                 │
│   └─ Infrastructure/    ← EF Core, JWT, file storage,        │
│                            middleware, DI, migrations         │
└──────────────────────────────────────────────────────────────┘
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
| Real-time          | **SignalR** hubs for chat and voice signaling                    |
| Logging            | **Serilog** — console + daily rolling files + request logging    |
| API documentation  | **Scalar** UI on native `Microsoft.AspNetCore.OpenApi`           |

## 🧰 Tech Stack

- **.NET 10** (C# 14, nullable + implicit usings enabled)
- ASP.NET Core Minimal APIs + SignalR
- Entity Framework Core 10 (SQL Server 2022)
- MediatR 14 · FluentValidation 12
- BCrypt.Net-Next · Serilog · Scalar
- Docker / Docker Compose

## 📡 API Surface

All REST routes live under `/api/v1`. Interactive docs: **`/scalar/v1`** (Development).

### Auth

| Method | Route | Description |
| ------ | ----- | ----------- |
| `POST` | `/auth/register` | Create account |
| `POST` | `/auth/login` | Login (sets cookies; `X-Skip-Cookies: true` for mobile) |
| `POST` | `/auth/refresh` | Rotate tokens (cookie or `X-Refresh-Token`) |
| `POST` | `/auth/logout` | Clear auth cookies |

### Users

| Method | Route | Description |
| ------ | ----- | ----------- |
| `GET` | `/users/me` | Current profile |
| `POST` | `/users/me/profile-image` | Upload avatar |
| `DELETE` | `/users/me/profile-image` | Remove avatar |

### Servers

| Method | Route | Description |
| ------ | ----- | ----------- |
| `GET` | `/servers` | Servers I belong to |
| `POST` | `/servers` | Create server (default channels + owner) |
| `POST` | `/servers/join` | Join by invite code |
| `GET` | `/servers/{id}` | Server details (members only) |
| `PATCH` | `/servers/{id}` | Rename (Admin+) |
| `DELETE` | `/servers/{id}` | Delete server (Owner) |
| `POST` | `/servers/{id}/leave` | Leave (not owner) |
| `POST` | `/servers/{id}/invite` | Regenerate invite code (Admin+) |
| `PUT` | `/servers/{id}/icon` | Upload icon (Admin+) |
| `DELETE` | `/servers/{id}/icon` | Remove icon (Admin+) |

### Members

| Method | Route | Description |
| ------ | ----- | ----------- |
| `GET` | `/servers/{id}/members` | List members |
| `PUT` | `/servers/{id}/members/{userId}/role` | Change role (Owner) |
| `DELETE` | `/servers/{id}/members/{userId}` | Kick member (Admin+ / Owner) |

### Channels

| Method | Route | Description |
| ------ | ----- | ----------- |
| `GET` | `/servers/{id}/channels` | List channels (members only) |
| `POST` | `/servers/{id}/channels` | Create text/voice channel (Admin+) |
| `DELETE` | `/servers/{id}/channels/{channelId}` | Delete channel + messages (Admin+) |

### Messages

| Method | Route | Description |
| ------ | ----- | ----------- |
| `GET` | `/channels/{id}/messages?beforeId=&take=` | History (cursor pagination, `take` ≤ 100) |
| `PUT` | `/channels/{id}/messages/{messageId}` | Edit own message |
| `DELETE` | `/channels/{id}/messages/{messageId}` | Delete (author or Admin+) |

### Real-time hubs

| Hub | Path | Client methods |
| --- | ---- | -------------- |
| Chat | `/hubs/chat` | `JoinChannel`, `LeaveChannel`, `SendMessage`, `Typing` |
| Voice | `/hubs/voice` | `JoinVoiceChannel`, `LeaveVoiceChannel`, `SendSignal` |

Server events include `ReceiveMessage`, `UserTyping`, `ExistingPeers`, `PeerJoined`, `PeerLeft`, `ReceiveSignal`.

Hubs accept the JWT as `?access_token=...` on the WebSocket handshake (or the auth cookie).

## 📁 Repository Structure

```
LigthSpeakBE/
├── LightSpeak/
│   ├── LightSpeak.slnx                   # Solution
│   └── LightSpeak.Backend/
│       ├── Common/                       # Result, interfaces, behaviors, ErrorTags
│       ├── Dominio/                      # Entities, enums, Entity base
│       ├── Features/                     # ← vertical slices
│       │   ├── Auth/                     #   login, register, refresh, logout
│       │   ├── Users/                    #   profile & avatar
│       │   ├── Servers/                  #   servers, members, invites, icons
│       │   ├── Channels/                 #   channel CRUD
│       │   ├── Messages/                 #   history & message CRUD
│       │   ├── Chat/                     #   ChatHub
│       │   └── Voice/                    #   VoiceHub
│       ├── Infrastructure/               # EF Core, JWT, storage, middleware, DI
│       │   └── Persistence/
│       │       ├── Configurations/       #   entity configurations
│       │       └── Migrations/           #   EF Core migrations
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

### Configuration

Secrets and environment-specific values live in a **git-ignored** `.env` file used by `docker-compose.yml` (connection string, JWT secret, CORS origin, file storage paths, log levels). Create your own local `.env` — never commit it.

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
| Chat hub             | `/hubs/chat`                 |
| Voice hub            | `/hubs/voice`                |

Interactive API documentation is available in Development via the Scalar UI.

> **Migrations:** new EF tools migrations should be generated with  
> `--output-dir Infrastructure/Persistence/Migrations`.

## 🗺️ Roadmap

- 🗂️ Soft delete for servers
- 🧪 Test suite
- 🔔 Notifications / presence
- 📱 Client apps (web / desktop)

## 📜 License

This project is **proprietary software**. All rights reserved by the copyright holder. See [LICENSE](./LICENSE) for details.

No part of this software may be reproduced, distributed, modified, or used in any form without prior written permission.

---

<div align="center">

Made with ❤️, ☕ and a lot of <code>dotnet watch</code>.

</div>
