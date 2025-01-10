# MindFlick

An over-engineered flashcard platform designed to help users better remember specific concepts and problems.

## Features:

- Bookmark Flashcards
- Create Public Flashcards
- Categorize Flashcards
- Free to Use
- Markdown Support

## Access the Platform

Visit [oop.hilmo.dev](https://oop.hilmo.dev)

## How to Set Up and Build

### Prerequisites

- .NET 8.0
- PostgreSQL database
- Google OAuth client ID and secret

### Docker Compose

**This compose file does not include database migration**
```yaml
services:
    mindflick:
        container_name: mindflick
        image: ghcr.io/hilmoo/mindflick:master
        restart: unless-stopped
        ports:
        - 8080:8080
        env_file:
        - .env
        depends_on:
            mindflick-db:
                condition: service_healthy
    mindflick-db:
        container_name: mindflick-db
        image: postgres:16-alpine
        restart: unless-stopped
        healthcheck:
          test: ["CMD-SHELL", "pg_isready -d $${POSTGRES_DB_NAME} -U $${POSTGRES_USER}"]
          start_period: 20s
          interval: 30s
          retries: 5
          timeout: 5s
        environment:
        - POSTGRES_USER=${POSTGRES_USER}
        - POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
        - POSTGRES_DB=${POSTGRES_DB_NAME}
        volumes:
        - ./database:/var/lib/postgresql/data
```

### Running Locally

1. Create or update your `.env` file with the necessary environment variables. You can check the `.env.example` file for
   a template.
2. Apply the database migrations:
   ```bash
   export DATABASE_URL="Host=<YOUR_POSTGRES_HOST>;Database=<YOUR_POSTGRES_DB_NAME>;Username=<YOUR_POSTGRES_USER>;Password=<YOUR_POSTGRES_PASSWORD>"
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
3. Build the application:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

### Running in Development Mode

1. Install npm if you haven't already.
2. Install the required npm packages:
   ```bash
   npm install
   ```
3. Update the `.env` file with the necessary values, using the `.env.example` file as a guide.
4. Apply the database migrations:
   ```bash
   export DATABASE_URL="Host=<YOUR_POSTGRES_HOST>;Database=<YOUR_POSTGRES_DB_NAME>;Username=<YOUR_POSTGRES_USER>;Password=<YOUR_POSTGRES_PASSWORD>"
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. Start the Tailwind CSS build process:
   ```bash
   npm run tw
   ```
6. Start the development server:
   ```bash
   npm run dev
   ```
