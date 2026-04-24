# Shop App

Student full-stack e-commerce project built for learning and portfolio use.

## Project status

In progress.

Core product and category flows are implemented, but the application is not finished yet.

## What the project currently does

- Public product listing and product detail views
- Product filtering by category
- Admin product management (create, read, update, delete)
- Category read operations
- Backend API with database persistence
- Unit tests for service layer behavior

## Tech Stack

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core (MySQL provider)
- RESTful API architecture
- DTO-based design
- Global exception handling middleware (ProblemDetails)
- C# async/await patterns

### Database
- MySQL

### Frontend
- Vanilla JavaScript (no framework)
- HTML5
- SCSS / CSS

### Testing
- xUnit

### Tooling
- Makefile (development automation)
- EF Core migrations

## Architecture summary

The project is split into two main parts:

- Frontend: static pages and JavaScript logic for public and admin interfaces
- BackendApi: REST API, business services, and persistence layer

Backend layers:

- Controllers: HTTP endpoints
- Services: business logic
- Data: EF Core DbContext and migrations
- DTOs: API contract objects
- Models: domain entities

## Local setup

### Prerequisites

- .NET 8 SDK
- MySQL
- Node.js (for Sass compilation)
- Sass CLI

### 1) Clone and enter project

```bash
git clone <https://github.com/BryanBD16/shop-app.git>
cd shop-app
```

### 2) Configure database connection

Set your MySQL connection string in:

- BackendApi/appsettings.json

### 3) Apply database migrations

```bash
cd BackendApi
dotnet ef database update
cd ..
```

### 4) Run backend API

```bash
cd BackendApi
dotnet run
```

Or from the project root:

```bash
make backend
```

### 5) Compile frontend styles

```bash
make sass
```

### 6) Open frontend pages

Open pages from the Frontend folder in your browser (public and admin pages are separated).

## Testing

Run all tests:

```bash
dotnet test
```

Run a specific test group:

```bash
dotnet test BackendApi.Tests/BackendApi.Tests.csproj --filter "CategoryServiceTests"
```

## Project structure

```text
shop-app/
├── BackendApi/         # API, services, EF Core context, models, migrations
├── BackendApi.Tests/   # Unit and integration tests
├── Frontend/           # Static pages, JS, SCSS/CSS
├── Makefile            # Dev shortcuts
└── shop-app.sln        # Solution file
```

## Development workflow

### Branching strategy

This project follows a simple feature-branch workflow:

- `main` → stable version
- `feature/*` → new features or improvements
- `fix/*` → bug fixes

Each GitHub issue is developed in its own branch.

Example:
```bash
feature/product-discount
fix/category-delete-bug
```

### Commit convention

This project follows [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/).

Format :
```bash
type(scope): description
```

Example :
```bash
feat(product): add discount system
fix(category): prevent deletion when products exist
refactor(api): simplify error handling with middleware
```




## Current limitations

- The application is not finished yet
- Some features are still being built and refined

## Roadmap (next steps)

- Complete remaining admin and public workflows
- Improve validation and error handling coverage
- Expand integration tests
- Improve deployment and environment documentation