# Architecture — Icon Task Management

This document walks through how the project is put together, what each piece does, and why it was built that way. It's written so you can read it once and confidently answer questions about any part of the system.

---

## High-level overview

The app is split into two independent deployable units:

- **Back-end** — A .NET 10 Web API that handles all business logic, data storage, and authentication.
- **Front-end** — A React 19 SPA (TypeScript + Vite) that talks to the API over HTTP.

They run on separate ports (back-end on 5000, front-end on 3000) and communicate through a REST API. Authentication tokens live in HTTP-only cookies, so the front-end never touches a JWT string directly.

When deployed via Docker, both services run in containers. The front-end's nginx reverse-proxies `/api/` requests to the back-end container, so everything goes through a single domain from the browser's perspective.

---

## Back-end architecture

The back-end follows **Clean Architecture** using four projects plus a shared kernel:

```
Icon.Api              → The HTTP entry point (controllers, middleware, config)
Icon.Application      → Use cases (commands, queries, validators)
Icon.Domain           → Entities, value objects, enums, domain events
Icon.Infrastructure   → Database access, Identity, JWT services
Icon.SharedKernel     → Interfaces and base types shared across all layers
```

Dependencies point inward: `Api` → `Application` → `Domain` ← `Infrastructure`. The Domain layer depends on nothing except SharedKernel. Infrastructure implements the abstractions that Application defines.

### Why this structure

The point is to keep business rules (Domain + Application) independent of how data is stored or how HTTP works. If tomorrow you swap SQLite for Postgres, you change one project (Infrastructure) and nothing else breaks. If you add a gRPC endpoint, you add another thin entry-point project alongside Api and reuse all the same commands and queries.

---

## How a request flows through the back-end

Let's trace what happens when the front-end calls `POST /api/v1/tickets` to create a ticket:

### 1. Controller receives the HTTP request

`TicketController.CreateTicket()` picks it up. The controller doesn't contain business logic — it just maps the HTTP request body (`CreateTicketRequest`) into a command object (`CreateTicketCommand`) and sends it to MediatR.

```csharp
var command = new CreateTicketCommand { Title = request.Title, ... };
var result = await mediator.Send(command, cancellationToken);
return this.ToActionResult(result, HttpStatusCode.Created);
```

### 2. Validation pipeline intercepts it

Before the handler runs, `ValidationPipelineBehavior` kicks in. It's a MediatR pipeline behavior that collects all `IValidator<TRequest>` implementations registered for this command type (in this case `CreateTicketCommandValidator`) and runs them.

If validation fails, the pipeline short-circuits and returns a `Result.Fail(...)` with `VALIDATION_ERROR` codes. The handler never executes.

This is how every command/query gets validated — there's no manual `if (!ModelState.IsValid)` anywhere. The behavior is registered once in `CoreModule` and applies to everything.

### 3. Handler executes the business logic

`CreateTicketCommandHandler.Handle()` runs:
- Gets the current user's ID from `IUserContextAccessor`
- Checks the repository for a duplicate title (`GetByTitleAsync`)
- Creates value objects (`TicketTitle`, `TicketDescription`)
- Calls the `Ticket.Create()` factory method (which raises a `TicketCreatedDomainEvent`)
- Adds the ticket via `ITicketRepository.AddAsync()`
- Saves via `IUnitOfWork.SaveChangesAsync()`

Every handler follows this same pattern: fetch → validate business rules → mutate → save.

### 4. Result gets mapped to an HTTP response

Back in the controller, `this.ToActionResult(result)` is an extension method that looks at the `Result<T>` and figures out the right HTTP status code. It inspects the error codes:
- Error code ending with `_NOT_FOUND` → 404
- `INVALID_CREDENTIALS` → 401
- `FORBIDDEN` → 403
- Has exceptions → 500
- Otherwise → 400

Every API response has the same shape:

```json
{
  "data": ...,
  "isSuccess": true/false,
  "isFailed": true/false,
  "errors": { "ERROR_CODE": "Human-readable message" }
}
```

This is the `ApiResponse<T>` record.

---

## Domain layer (Icon.Domain)

### Ticket entity

This is the core aggregate root. It has:
- **Value objects**: `TicketTitle` and `TicketDescription` — they do their own validation and are immutable.
- **Enums**: `TicketStatus` (Open, InProgress, InReview, Done, Closed) and `TicketPriority` (Low, Medium, High, Critical).
- **Domain events**: `TicketCreatedDomainEvent` and `TicketStatusChangedDomainEvent` — raised when things happen and can be subscribed to by handlers.
- **Factory method**: `Ticket.Create(...)` is the only way to create a new ticket. The constructor is private.

Private setters enforce that state can only be changed through named methods (`UpdateDetails`, `AssignPriority`, `ChangeStatus`, `MarkAsCompleted`, `Reorder`). This prevents anything outside the entity from putting it into an invalid state.

### DomainEntity base class

Manages a list of domain events. Entities can call `RaiseDomainEvent()` and later the events can be dispatched (though in this project they're raised for completeness — no handlers consume them yet).

### Specifications

The specification pattern is used for building queries. `TicketsByUserSpecification` defines the filter criteria (user ID, completion status, search text) and sorting, and the repository applies it to the database query. This keeps query logic out of the handlers and repositories clean.

---

## Application layer (Icon.Application)

### CQRS with MediatR

Commands change data. Queries read data. Both are dispatched through MediatR.

Commands:
- `CreateTicketCommand` → returns `Result<Ulid>` (the new ID)
- `UpdateTicketCommand` → returns `Result<bool>`
- `DeleteTicketCommand` → returns `Result<bool>`
- `ChangeTicketStatusCommand` → returns `Result<bool>`
- `CompleteTicketCommand` → returns `Result<bool>`
- `ReorderTicketsCommand` → returns `Result<bool>`
- `LoginCommand`, `RegisterCommand`, `LogoutCommand`, `RefreshTokenCommand`

Queries:
- `GetTicketsQuery` → returns `Result<GetTicketsResponse>` (list + envelope)
- `GetTicketByIdQuery` → returns `Result<TicketDetailResponse>`
- `GetCurrentUserQuery` → returns `Result<AuthenticationResponse>`

### Feature folders

Each feature is self-contained in its own folder:

```
Features/
  Tickets/
    CreateTicket/
      CreateTicketCommand.cs
      CreateTicketCommandHandler.cs
      CreateTicketCommandValidator.cs
```

This makes it easy to find everything related to a specific operation. You don't jump between "Commands", "Handlers", and "Validators" folders.

### FluentResults

Instead of throwing exceptions for expected failures (user not found, duplicate title, etc.), handlers return `Result.Fail(...)` with a `CustomFluentError` that carries an error code and a message. This way the controller can map each error type to the right HTTP status code without try/catch blocks.

### Validation

Every command/query that needs validation has a FluentValidation validator class. The `ValidationPipelineBehavior` picks them up automatically (they're registered by scanning the assembly in `CoreModule`). Validation errors use the `VALIDATION_ERROR` code.

---

## Infrastructure layer (Icon.Infrastructure)

### EF Core + SQLite

`ApplicationDbContext` extends `IdentityDbContext` (because the same database stores both Identity tables and the Tickets table). Entity configurations are applied from the assembly via `ApplyConfigurationsFromAssembly`.

The `EfRepository<TEntity, TPrimaryKey>` is a generic repository implementation. It applies specifications using the specification evaluator, which translates `ISpecification` properties (Criteria, Includes, OrderExpressions, etc.) into LINQ queries.

### Unit of Work

`UnitOfWork` wraps `DbContext.SaveChangesAsync()`. It also supports explicit transactions via `TransactionScope`. All handlers that modify data call `_unitOfWork.SaveChangesAsync()` at the end — no auto-saving.

### Database initialization

On startup, `DatabaseInitializer.InitializeAsync()` calls `EnsureCreatedAsync()`. This creates the SQLite file and all tables if they don't exist. No migration history is tracked (it's `EnsureCreated`, not `Migrate`), which is simpler for a project this size.

---

## Authentication

### How it works

Authentication is cookie-based JWT. Here's the flow:

1. **Login**: User sends email + password → handler verifies via ASP.NET Identity's `SignInManager` → on success, generates a JWT access token and a refresh token → writes both into HTTP-only, Secure, SameSite cookies.

2. **Subsequent requests**: The `JwtBearerEvents.OnMessageReceived` callback reads the access token from the cookie (not from an Authorization header). The standard JWT middleware validates it.

3. **Token refresh**: When the access token expires, the front-end calls `/auth/refresh`. The handler reads the expired access token from the cookie, extracts the user ID, generates new tokens, and overwrites the cookies.

4. **Logout**: Clears both cookies.

### Why cookies instead of localStorage

Cookies with `HttpOnly` and `Secure` flags can't be accessed by JavaScript, which protects against XSS attacks stealing tokens. The front-end doesn't need to manage tokens at all — the browser sends them automatically.

### Identity setup

ASP.NET Identity handles password hashing, lockout policies, and user storage. It's configured in `AuthenticationModule` with requirements like minimum 8-character passwords, uppercase, lowercase, digit, and special character. Account lockout happens after 5 failed attempts for 15 minutes.

### JWT configuration

- Access token expires in 15 minutes
- Refresh token expires in 7 days
- Tokens are validated with issuer, audience, signing key, and zero clock skew
- The `X-Token-Expired` header is set when authentication fails due to expiration, so the front-end's interceptor knows to attempt a refresh

---

## API layer (Icon.Api)

### Modules

Dependency injection is organized into three modules:
- `CoreModule` — Registers MediatR, FluentValidation validators, and the validation pipeline behavior.
- `DataModule` — Registers EF Core, the DbContext pool, repositories, and the unit of work.
- `AuthenticationModule` — Registers Identity, JWT Bearer authentication (including the cookie-reading callback), and auth-related services.

Each module implements `IModule.Load(IServiceCollection)`, and they're registered in `Program.cs` with `builder.Services.AddModule(new ...)`.

### Middleware

There's one custom middleware component:
- `GlobalExceptionHandler` — Catches unhandled exceptions using the `IExceptionHandler` pattern. Returns `ArgumentException` as 400, everything else as 500, always in the standard `ApiResponse` JSON shape.

### API versioning

Routes are versioned: `/api/v1/tickets`. This is configured through `Asp.Versioning` with URL-segment versioning. If a v2 is ever needed, you add a new controller with `[ApiVersion("2")]` and both versions coexist.

### Custom model binding

`UlidModelBinderProvider` handles converting string route parameters to `Ulid` values so the controllers can accept `Ulid id` directly.

---

## Front-end architecture

### Stack

- React 19 with TypeScript
- Vite for dev server and builds
- Tailwind CSS v4 for styling
- dnd-kit for drag-and-drop ticket reordering
- Zod for form validation
- Axios for API calls
- React Context API for state management (no Redux)

### Structure

```
src/
  api/            → Axios instance + service modules (authService, ticketService)
  components/
    auth/         → LoginModal, RegisterModal
    layout/       → Layout (header, nav)
    tickets/      → TicketBoard, TicketCard, TicketForm, etc.
    ui/           → Reusable primitives (Button, Modal, etc.)
  context/
    AuthContext    → Auth state, login/register/logout functions
    TicketContext  → Ticket state, CRUD operations, filtering
  hooks/
    useAuth       → Shortcut to AuthContext
    useTickets    → Shortcut to TicketContext
    useForm       → Generic form handling with Zod validation
  pages/
    Dashboard     → The main (and only) page
  types/          → TypeScript interfaces matching the API contracts
```

### State management

Two React Contexts drive the app:

**AuthContext** handles the full auth lifecycle:
- On mount, it calls `GET /auth/me` to check if the user is already logged in (cookie is present).
- Provides `login()`, `register()`, `logout()`, `refreshUser()`.
- Listens for a custom `auth:unauthorized` event (dispatched by the Axios interceptor on 401) to show the login modal.

**TicketContext** handles ticket data:
- Fetches tickets whenever the user is authenticated or the filter changes.
- `reorderTickets()` does optimistic updates — it updates local state immediately for smooth drag-and-drop, then sends the request. If the API call fails, it re-fetches.
- `getTicketsByStatus()` filters and sorts tickets for the Kanban board columns.

### API layer

The Axios instance is configured with `withCredentials: true` (so cookies are sent cross-origin) and a response interceptor that:
1. On 401, tries to call `/auth/refresh` once.
2. If refresh succeeds, replays the original request.
3. If refresh fails, fires the `auth:unauthorized` event so the AuthContext shows the login modal.

This means token refresh is completely invisible to the user.

### Form handling

`useForm` is a custom hook that takes a Zod schema and handles field-level validation on blur, full validation on submit, and provides `isSubmitting` and `serverError` state. It avoids the weight of a full form library while still covering real-time validation.

### Drag and drop

The ticket board uses dnd-kit. Each status column is a droppable area, and each ticket card is draggable. When a ticket is dropped, the reorder command sends the new `sortOrder` values to the API. The board groups tickets by status and sorts by `sortOrder` within each group.

---

## Testing

Unit tests live in `Icon.Tests.Unit` and use:
- **xUnit** as the test framework
- **Moq** for mocking interfaces (repositories, services)
- **FluentAssertions** for readable assertions
- **Bogus** for generating realistic test data

### Test organization

Tests follow the same feature-folder structure as the application:

```
Features/
  Tickets/
    CreateTicketTests.cs
    UpdateTicketTests.cs
    DeleteTicketTests.cs
    GetTicketByIdTests.cs
    GetTicketsTests.cs
    ChangeTicketStatusTests.cs
    CompleteTicketTests.cs
```

### Builder pattern

Each test file has a nested `Builder` class that sets up mocks with sensible defaults. You chain methods to override specific behavior:

```csharp
var builder = new Builder()
    .WithExistingTicket(ticket)    // GetByIdAsync returns this ticket
    .WithUserId("user-123")        // IUserContextAccessor.UserId returns this
    .WithSaveChanges(1);           // SaveChangesAsync returns 1 (success)
```

The builder exposes a `Handle()` method that creates the handler with all the mocked dependencies and calls it. This keeps individual test methods short and focused on the scenario being tested.

### What's covered

- Happy paths (creating, updating, deleting, completing, status changes)
- Ownership checks (trying to modify someone else's ticket → `FORBIDDEN`)
- Not-found cases
- Duplicate detection (duplicate ticket title)
- Invalid input (bad status strings, bad priority strings)
- Idempotency (completing an already-completed ticket)
- SaveChanges returning 0 (nothing actually saved)

---

## Docker setup

The `docker-compose.yml` runs two services:

| Service | Image | Port | Notes |
|---------|-------|------|-------|
| backend | Built from `back-end/Dockerfile` | 5000 | .NET runtime, SQLite stored in a named volume |
| frontend | Built from `front-end/Dockerfile` | 3000 (mapped from 80) | nginx serving the Vite build output |

The front-end Dockerfile is a two-stage build: Node builds the app, then the dist folder is copied into an nginx image. The nginx config (`nginx.conf`) serves static files and reverse-proxies `/api/` to the backend container.

The back-end Dockerfile is also two stages: SDK image restores and publishes, then the runtime image runs the DLL.

SQLite data is persisted in a Docker volume (`db-data`) mounted at `/data` inside the backend container.

---

## Key design decisions — quick reference

| Decision | Why |
|----------|-----|
| Clean Architecture | Keep business logic independent of frameworks and infrastructure |
| CQRS via MediatR | Separate read and write paths, easy to add cross-cutting behaviors |
| FluentResults instead of exceptions | Expected failures (not found, validation) shouldn't be exceptional |
| Specification pattern | Query logic is reusable and testable outside the repository |
| Value objects (TicketTitle, TicketDescription) | Self-validating, immutable wrappers prevent invalid state |
| Cookie-based JWT | More secure than localStorage — immune to XSS token theft |
| Feature folders | Everything for one use case lives together — easier to navigate |
| Optimistic reorder | Drag-and-drop feels instant; rolls back if the API fails |
| Zero-config SQLite | No database server to install. Works out of the box |
| Modules for DI | Registration logic grouped by concern, not scattered in Program.cs |
