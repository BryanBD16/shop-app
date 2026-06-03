# Shop App — Portfolio Project

This repository contains a full-stack e-commerce demo application created as a portfolio and learning project. It is intended to demonstrate web application concepts, backend API design, and frontend UI work. This project is not production-ready and is provided for demonstration and evaluation purposes only.

## Project status

In progress.

Core product, category and discount flows are implemented; some features and polishing are still under development.

## What the project currently does

- Public product listing and product detail views
- Product filtering by category
- Admin product management (create, read, update, delete)
- Category read operations
- Admin discount management (create, read, update, delete)
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
- `Frontend-Angular` — Angular (TypeScript) application used for the UI and admin interfaces. Run with `npm install` and `npm start` in the `Frontend-Angular` folder.
 
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

Backend prerequisites

- .NET 8 SDK (8.0+). Verify with:

```bash
dotnet --version
```

- MySQL server (8.0+). Verify with:

```bash
mysql --version
```

- Optional: EF Core CLI (`dotnet-ef`) for applying migrations. If you don't have it installed globally you can install it with:

```bash
dotnet tool install --global dotnet-ef
```

- `make` and `sudo` are used by some Makefile targets (e.g., `db-start`) to control the local MySQL service. These are optional — you can run the equivalent `dotnet` and service commands manually if you prefer.

Angular frontend prerequisites

- Node.js (LTS, recommended >=18) and `npm`. Verify with:

```bash
node --version
npm --version
```

- Global Angular CLI is NOT required. The project uses the local Angular CLI in `Frontend-Angular` devDependencies; run `npm install` then `npm start` from that folder.

- (Optional) If you prefer `yarn`, it will also work after installing dependencies with `yarn` in `Frontend-Angular`.

Note: The legacy `Frontend` folder is archived and not used by the current development workflow.


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

### 4b) Run the Angular frontend (optional)

Open a new terminal and run:

```bash
cd Frontend-Angular
npm install
npm start
```

The Angular app will run on the default `ng serve` port (usually `http://localhost:4200`).

### 5) (Optional) Build the Angular frontend for production

From the `Frontend-Angular` folder run:

```bash
npm run build
```

The production build will be output to `Frontend-Angular/dist` by default and can be served by a static file server or integrated into a hosting pipeline.

## Testing

Run all tests:

```bash
dotnet test
```

Run a specific test group:

```bash
dotnet test BackendApi.Tests/BackendApi.Tests.csproj --filter "CategoryServiceTests"
```

For frontend unit tests (Angular):

```bash
cd Frontend-Angular
npm test
```

## Project structure

```text
shop-app/
├── BackendApi/           # API, services, EF Core context, models, migrations
├── BackendApi.Tests/     # Unit and integration tests
├── Frontend-Angular/     # Angular (TypeScript) application used for modern UI (primary frontend)
├── Makefile              # Dev shortcuts
└── shop-app.sln          # Solution file
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