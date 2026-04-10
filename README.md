# Shop App

## Overview

This project is a web-based e-commerce application developed as part of a personal learning and portfolio initiative.

The application currently provides core product management features, including a full CRUD interface accessible through an admin panel. Products can be created, updated, and managed by administrators, and are made available for viewing through a separate user-facing interface when marked as available.

The primary objective of this project is educational. It is designed to support a complete end-to-end understanding of web application development, including backend API design, frontend integration, data management, and deployment considerations.

This project also serves as a portfolio piece to demonstrate practical skills, technical decision-making, and the ability to design and implement a functional web application from scratch.

---

## Architecture

### General Overview

The application follows a client-server architecture with a clear separation of concerns between the frontend, backend, and data layers.

The backend is implemented as a RESTful API using C# and ASP.NET Core. It follows a layered architecture composed of controllers, services, and data access components. Controllers handle HTTP requests and responses, services encapsulate business logic, and Entity Framework Core is used to manage database interactions through a DbContext and migrations system. Data Transfer Objects (DTOs) are used to structure and control the data exchanged between layers and exposed through the API.

The frontend is built using JavaScript, HTML, and SCSS. It is responsible for rendering the user interface and interacting with the backend API via HTTP requests. The application includes separate views for administrative and user functionalities, allowing product management (CRUD operations) on the admin side and product browsing on the user side.

Data is persisted in a MySQL relational database, managed locally during development. Database schema evolution is handled using Entity Framework Core migrations, ensuring consistency between the application models and the database structure.

The overall architecture emphasizes modularity, maintainability, and clarity, making it suitable for learning purposes while reflecting common patterns used in real-world web applications.

### Backend (C# API)
TODO:
- Framework used (e.g., ASP.NET Core)
- Project structure
- Key components (controllers, services, repositories)
- Design patterns used

### Frontend (JavaScript)
TODO:
- Framework or library used (Angular, React, etc.)
- Project structure
- State management (if applicable)
- Routing

### Communication
TODO:
- Type of communication (REST API)
- Data format (JSON)
- Authentication method (if applicable)

---

## Design Decisions
TODO: Explain important technical decisions and trade-offs.

- Decision 1: TODO
- Decision 2: TODO
- Decision 3: TODO

---

## Data Management

### Models
TODO: Describe core data models (e.g., Product, User).

### Persistence
TODO: Explain how data is stored (database, in-memory, external API).

### Data Flow
TODO: Describe how data flows between frontend and backend.

---

## Technologies Used

### Backend
- TODO: .NET / ASP.NET Core
- TODO: Additional libraries

### Frontend
- TODO: Framework (Angular, React, etc.)
- TODO: Additional libraries

### Dev Tools
- TODO: Git
- TODO: Docker (if used)
- TODO: Other tools

---

## Project Structure
```bash
TODO: Provide a tree structure of the project