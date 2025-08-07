# TrackExpense

TrackExpense is a backend-focused .NET 9 Web API project designed for tracking personal income and expenses. It follows a layered Clean Architecture approach and leverages Entity Framework Core for data persistence. The goal is to keep the codebase modular, testable, and maintainable.

---

## Features

- Track income and expenses by category
- Group expenses by category
- Calculate summaries over a selected period
- Account-level filtering

---

## Architecture

The project is based on Clean Architecture principles:

- **Domain Layer** — Contains core business logic and entities
- **Application Layer** — Contains interfaces and use cases
- **Infrastructure Layer** — Implements data access and external dependencies
- **API Layer** — HTTP interface, controllers

---

## Technologies

- .NET 9 / ASP.NET Core
- Entity Framework Core
- SQL Server
- NUnit

---

## Repository Structure

```bash
/TrackExpense
├── TrackExpense.Api            # ASP.NET Core Web API
├── TrackExpense.Application    # Interfaces and business logic
├── TrackExpense.Domain         # Domain entities and core logic
├── TrackExpense.Infrastructure # EF Core DbContext and implementations
└── TrackExpense.Tests          # Unit tests
```

---

## System Requirements

- .NET 9 SDK
- SQL Server (version 2017 or newer)
- Windows / Linux / macOS

---

## Setup and Run

1. **Clone the repository**:
   ```bash
   git clone https://github.com/thatshowmafiaworks/TrackExpense.git
   cd TrackExpense
   ```

2. **Configure `appsettings.json`**:  
   Open `appsettings.json` in the `TrackExpense.Api` project and set the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=TrackExpenseDb;Trusted_Connection=True;"
   },
   "JWT":{
     "SigninKey"  : "SuperSecretSigninKeyForTrackExpenseJwtTokenGenerator1234567890",
     "Issuer"     : "TrackExpense",
     "Audience"   : "TrackExpense"
   }
   ```

3. **Apply EF Core migrations**:
   ```bash
   cd TrackExpense.Infrastructure
   dotnet ef migrations add Init
   dotnet ef database update
   ```

4. **Run the API**:
   ```bash
   cd ../TrackExpense.Api
   dotnet run
   ```
---

## Testing

To run unit tests:

```bash
cd TrackExpense.Tests
dotnet test
```

---

###### Generated via ChatGpt, reviewed by me:)
