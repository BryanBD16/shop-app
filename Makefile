# ============================
# Configuration
# ============================

FRONTEND_DIR=Frontend
BACKEND_DIR=BackendApi

# ============================
# Phony targets
# ============================

.PHONY: sass backend dev db-drop db-start db-stop migrate


# ============================
# Sass compilation (once)
# ============================

sass:
	@echo "Compiling SCSS → CSS"
	cd $(FRONTEND_DIR) && sass scss/style.scss css/style.css

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
	make -j2 sass backend

# ============================
# MySQL + Entity Framework Core
# ============================

# Nom du projet backend (le .csproj)
PROJECT=BackendApi.csproj

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

# Apply all migrations to the database
migrate:
	@echo "Applying EF Core migrations..."
	dotnet ef database update --project $(PROJECT)

# Drop the database
db-drop:
	@echo "Dropping the database..."
	dotnet ef database drop --project $(PROJECT) --force

# ============================
# Notes
# ============================

# To add a new migration:
#   dotnet ef migrations add <MigrationName> --project $(PROJECT)
# Example:
#   dotnet ef migrations add InitialCreate --project $(PROJECT)




