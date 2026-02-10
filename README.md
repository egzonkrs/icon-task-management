# Icon - Simple Task Management System

**Live here:** [https://icon-tm-b5djbraadfaefpdj.westeurope-01.azurewebsites.net](https://icon-tm-b5djbraadfaefpdj.westeurope-01.azurewebsites.net) 🔴

A ticket management app with a .NET back-end API and a React front-end.
Users can sign up, log in, and then create / edit / delete / reorder tickets.
Authentication is cookie-based (JWT stored in HTTP-only cookies).

---

## Prerequisites

| Tool | Version |
|------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10 (preview) |
| [Node.js](https://nodejs.org/) | 20+ |
| npm | comes with Node |

> Docker is only needed if you want to run through containers (see below).

---

## Running locally (without Docker)

### 1. Back-end

```bash
cd back-end/src/Icon.Api
dotnet run
```

The API starts on **http://localhost:5000** by default.
Swagger UI is available at **http://localhost:5000/swagger** while running in Development mode.

The database is SQLite — a file called `icon.db` is created automatically on first run, and migrations are applied on startup so there is nothing extra to set up.

### 2. Front-end

```bash
cd front-end
npm install
npm run dev
```

Vite starts the dev server on **http://localhost:3000**.

The front-end expects the API at the URL defined in `front-end/.env`:

```
VITE_API_BASE_URL=http://localhost:5000/api/v1
```

Change this if you run the API on a different port.

---

## Running with Docker

From the project root:

```bash
docker compose up --build
```

This starts two containers:

| Service | URL |
|---------|-----|
| Back-end API | http://localhost:5000 |
| Front-end | http://localhost:3000 |

The SQLite database is persisted in a Docker volume (`db-data`), so data survives container restarts.

To stop everything:

```bash
docker compose down
```

---

## API overview

All API routes are versioned under `/api/v1`. Here is a quick reference:

### Auth

| Method | Path | What it does |
|--------|------|--------------|
| POST | `/api/v1/auth/register` | Create a new account |
| POST | `/api/v1/auth/login` | Log in (sets auth cookies) |
| POST | `/api/v1/auth/logout` | Log out (clears cookies) |
| GET | `/api/v1/auth/me` | Get the current user's info |

### Tickets

| Method | Path | What it does |
|--------|------|--------------|
| GET | `/api/v1/tickets` | List tickets (supports search, pagination) |
| GET | `/api/v1/tickets/{id}` | Get a single ticket |
| POST | `/api/v1/tickets` | Create a ticket |
| PUT | `/api/v1/tickets/{id}` | Update a ticket |
| DELETE | `/api/v1/tickets/{id}` | Delete a ticket |
| PATCH | `/api/v1/tickets/{id}/status` | Change a ticket's status |
| POST | `/api/v1/tickets/{id}/complete` | Mark a ticket as completed |
| PUT | `/api/v1/tickets/reorder` | Reorder tickets (drag-and-drop) |

Full request/response details are in the Swagger docs at `/swagger` when running in Development mode.

---

## Project structure

```
back-end/
  src/
    Icon.Api            → ASP.NET entry point, controllers, middleware
    Icon.Application    → Use cases, command/query handlers (MediatR)
    Icon.Domain         → Entities, value objects, domain events
    Icon.Infrastructure → Database context, Identity, JWT implementation
    Icon.SharedKernel   → Shared interfaces and base classes
  tests/
    Icon.Tests.Unit     → Unit tests

front-end/
  src/
    api/        → Axios client and API calls
    components/ → Reusable UI components
    context/    → React context providers
    hooks/      → Custom hooks
    pages/      → Route pages
    types/      → TypeScript types
```

---

## Running tests

```bash
cd back-end
dotnet test
```
