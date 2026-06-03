# ============================
# Configuration
# ============================

FRONTEND_DIR=Frontend-Angular
BACKEND_DIR=BackendApi

# ============================
# Phony targets
# ============================

.PHONY: sass backend dev db-drop db-start db-stop migrate

# ============================
# Frontend (Angular)
# ============================
frontend:
	@echo "Starting frontend (Angular)"
	cd $(FRONTEND_DIR) && ng serve

# ============================
# Backend (.NET)
# ============================

backend:
	@echo "Starting backend (.NET)"
	cd $(BACKEND_DIR) && dotnet run

# ============================
# Full dev environment
# ============================

dev: db-start
	@echo "Starting full development environment"
	make -j2 frontend backend

# ============================
# MySQL control
# ============================

# Start MySQL service
db-start:
	@echo "Starting MySQL service..."
	sudo service mysql start

# Stop MySQL service
db-stop:
	@echo "Stopping MySQL service..."
	sudo service mysql stop

# ============================
# Entity Framework Core commands
# ============================

# Nom du projet backend (le .csproj)
PROJECT=BackendApi/BackendApi.csproj

# Apply all migrations to the database
migrate-up:
	@echo "Applying EF Core migrations..."
	dotnet ef database update --project $(PROJECT)

	# make migrate-add NAME=MigrationName
migrate-add:
	dotnet ef migrations add $(NAME) --project $(PROJECT)

# Drop the database
db-drop:
	@echo "Dropping the database..."
	dotnet ef database drop --project $(PROJECT) --force

db-reset: db-drop migrate-up

migrate-drop:
	@echo "Dropping all migrations..."
	dotnet ef migrations remove --project $(PROJECT) --force

# ============================
# Tests commands
# ============================

# Run all tests
test:
	dotnet test

# ============================
# Notes
# ============================




