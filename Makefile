# ============================
# Configuration
# ============================

FRONTEND_DIR=Frontend
BACKEND_DIR=BackendApi

# ============================
# Phony targets
# ============================

.PHONY: sass backend dev db-init db-start db-stop


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
# MySQL control
# ============================

DB_SCHEMA_FILE=database/schema.sql

# Initialize the database
db-init: db-start
	@echo "Initializing database..."
	mysql -u root -p < $(DB_SCHEMA_FILE)

# Start MySQL service
db-start:
	@echo "Starting MySQL service if not already running..."
	sudo service mysql start

# Stop MySQL service
db-stop:
	@echo "Stopping MySQL service..."
	sudo service mysql stop



