# Tv-Maze API 

## Description

This ASP.NET Core application exposes an API that interacts with the TV Maze API to store and query TV show information.

## Arquitecture

- **Codere_Challenge_Api**: Controllers that expose endpoints.
- **Codere_Challenge_Services**: Services that contain business logic.
- **Codere_Challenge_Core**: Entities and contracts (repository interfaces).
- **Codere_Challenge_Infrastructure**: Database context and repository implementations.
- **Codere_Challenge_Jobs**: Job service that fetch data from API and store it in Db.

## Tests

- **Codere_Challenge_Api_Unit_Tests**: Unit Tests for controller endpoints.
- **Codere_Challenge_Jobs_Unit_Tests**: Unit tests for job methods.
- **Codere_Challenge_Services**: Unit tests for service methods.

## Prerequisites
- Install sqlite3
- Follow this tutorial link: https://www.sqlitetutorial.net/download-install-sqlite/

## Configuration

1. Configure the ConnectionString in `appsettings.json`.
2. Execute migrations to create database:
   ```bash
   dotnet ef --project Codere_Challenge_Infrastructure --startup-project Codere_Challenge_Api migrations add Initial
   dotnet ef --project Codere_Challenge_Infrastructure --startup-project Codere_Challenge_Api database update
3. Execute the following command:
   ```bash
   dotnet run

This command will expose the following api url: https://127.0.0.1:7128/

4. Open Postman, create the following Urls and configs:

- POST: https://127.0.0.1:7128/api/Job/run-fetch-shows, Headers: Key=x-api-key, Value=x_api_key

- POST: https://127.0.0.1:7128/api/Shows/Filter
Body: 
{
  "name": "Under"
}

- GET: https://127.0.0.1:7128/api/Job/GetAllExecutions, Headers: Key=x-api-key, Value=x_api_key

5. Execute the following command to stop dotnet process:
   ```bash
    get-process -name "dotnet" | stop-process

## Unit Tests

- **Codere_Challenge_Api_Unit_Tests**: Unit Tests for controller endpoints.
   ```bash
   cd Codere_Challenge_Api_Unit_Tests
   dotnet test

- **Codere_Challenge_Jobs_Unit_Tests**: Unit tests for job methods.
   ```bash
   cd Codere_Challenge_Jobs_Unit_Tests
   dotnet test

- **Codere_Challenge_Services**: Unit tests for service methods.
   ```bash
   cd Codere_Challenge_Services
   dotnet test