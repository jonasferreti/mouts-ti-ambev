# Developer Evaluation Project - Mouts Ti (Ambev)

This repository contains the solution for the developer evaluation technical test, focusing on **Domain-Driven Design (DDD)** principles, Unit Testing, and demonstrating knowledge of modern C# practices, **CQRS**, and **.NET 8**.

---

## 🚀 Overview

The core objective of this project was to implement the **Sales Domain** model, ensuring **transactional consistency** within the Aggregate Root and enforcing all business invariants. The solution expands into the **Application Layer** utilizing **CQRS** for separation of concerns and scalability.

> ⚠️ Note: The API does **not** implement authentication due to the scope of this bounded context. In a real-world scenario, there would be a dedicated authentication API that each microservice would consume.

### Key Deliverables

* **Domain Model:** Implementation of the `Sale` Aggregate Root and the `SaleItem` Entity.  
* **CQRS Modeling:** Separation of concerns using **Commands** and **Queries**.  
* **Data Processing:** Data handling and orchestration with **MediatR** and **AutoMapper**.  
* **Redis Caching:** Implementation of **Redis** for caching in the sales queries.  
* **Asynchronous Communication:** Domain events handled via **Rebus In-Memory**, with consumers logging via **Serilog** for:  
  `SaleCreatedEvent`, `SaleCancelledEvent`, `SaleItemCancelledEvent`, `SaleModifiedEvent`, `SaleDeletedEvent`, `SaleItemDeletedEvent`.  
* **Cache Management:** Cache invalidation managed via write/delete events.  
* **Unit Tests:** Comprehensive test coverage with **xUnit**, **NSubstitute** and **Faker**.  

---

## 🛠️ Technology Stack

| Category | Tool / Framework | Purpose |
| :--- | :--- | :--- |
| **Framework** | **.NET 8 (LTS)** | Primary runtime and core libraries |
| **Architecture** | **MediatR** | CQRS pattern and message dispatching |
| **Messaging** | **Rebus In-Memory** + **Serilog** | Event bus and logging |
| **Caching** | **Redis** | Distributed caching layer |
| **Containerization** | **Docker** & **docker-compose** | Packaging and orchestration |
| **Testing** | **xUnit** | Unit testing framework |
| **Mocking** | **NSubstitute** | Mocks and stubs |
| **Test Data** | **Faker (Bogus)** | Realistic test data generation |

---

## 💾 Database Choice: PostgreSQL

Choosing **PostgreSQL** as the primary persistence layer is a strategic decision perfectly aligned with the transactional demands of the Sales Domain.

### Why PostgreSQL?

- **Domain Modeling (1:N):**  
  Its relational nature is ideal for modeling the Sales domain, which features a 1:N relationship between the Sale Aggregate Root and its SaleItems. It inherently guarantees the native referential integrity required by DDD.

- **Transactional Consistency (ACID):**  
  Robust support for ACID properties ensures all modifications to the Sale Aggregate Root are executed as a single, atomic transaction, maintaining domain invariants and overall data consistency.

- **High EF Core Compatibility:**  
  Offers high-performance, seamless integration with Entity Framework Core (EF Core) via the Npgsql provider, delivering a modern and reliable persistence layer within the .NET ecosystem.

---

## 📂 Project Structure

```
├── Solution 'Ambev.DeveloperEvaluation'
│   ├── Adapters
│   │   ├── Driven
│   │   │   └── Infrastructure
│   │   └── Drivers
│   │       └── WebApi
│   ├── Core
│   │   ├── Application
│   │   │   ├── Dependencies
│   │   │   ├── Common
│   │   │   ├── Consumers
│   │   │   ├── Exceptions
│   │   │   ├── Sales # Commands, Queries, Handlers
│   │   │   └── Users
│   │   └── Domain
│   │       ├── Common
│   │       ├── Entities # Sale (Aggregate Root), SaleItem 
│   │       ├── Repositories # Interfaces
│   │       └── ValueObjects # Money, Quantity, ExternalReference
│   ├── Crosscutting
│   │   ├── Ambev.DeveloperEvaluation.Common
│   │   │   ├── Cache
│   │   └── Ambev.DeveloperEvaluation.IoC
│   └── Tests
```

---

## ✅ Unit Test Coverage Summary

### Domain Layer

#### Sale Aggregate Root (`Sale`)

- **Item Management:** Methods `AddItem`, `RemoveItem`, and `Update` correctly recalculate the `TotalAmount`.
- **Cancellation Rules:** Supports cascading cancellation; throws `DomainException` for invalid operations.

#### SaleItem Entity (`SaleItem`)

- **Discount Logic:** Supports 0%, 10%, and 20% discounts validated by quantity-based rules.

#### Value Objects (`Money`, `Quantity`, `ExternalReference`)

- **Invariants:** Enforce positive values, maximum quantity limits, and null-safety validations.

---

### Application Layer

#### Handlers, Commands & Queries

- **Orchestration & Validation:** Unit tests cover application layer handlers, commands, and queries to ensure business logic is correctly orchestrated and validated.
- **Event Dispatching:** Tests verify that domain/application events are published as expected (e.g., `SaleCreatedEvent`, `SaleCancelledEvent`).


---

## ⚙️ Local Configuration, Secrets, and HTTPS

Before running the application with `docker-compose`, configure the HTTPS certificate, secrets, and apply the EF migrations.

### 1. Configure HTTPS Certificate

Since the `docker-compose` mounts the local HTTPS certificate volume `~/.aspnet/https`, you must create the certificate and trust it locally:

```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p ev@luAt10n
dotnet dev-certs https --trust
```

---

### 2. Configure .NET User-Secrets

The application uses **.NET User-Secrets** for storing sensitive data like connection strings.

Initialize secrets (if not already):

```bash
dotnet user-secrets init
```

Set required secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=ambev.developerevaluation.database;Database=developer_evaluation;User Id=developer;Password=ev@luAt10n;TrustServerCertificate=True"
dotnet user-secrets set "ConnectionStrings:RedisConnection" "ambev.developerevaluation.cache:6379,password=ev@luAt10n,abortConnect=False"
```

Verify:

```bash
dotnet user-secrets list
```

Ensure the volume mount is present in `docker-compose.yml`:

```
- ~/.aspnet/https:/https:ro
```

---

### 3. Apply Database Migrations

After configuring secrets, navigate to the **ORM** project and apply migrations using a **relative path**:

```bash
cd src/Ambev.DeveloperEvaluation.ORM
dotnet ef database update --startup-project ../Ambev.DeveloperEvaluation.WebApi/Ambev.DeveloperEvaluation.WebApi.csproj
```

---

### 4. Run the Application with Docker

Build and start all services:

```bash
docker compose up --build -d
```

The application will be available at:

👉 **https://localhost:8081/swagger/index.html**

---

### 5. Run Unit Tests

Run all tests using the solution file:

```bash
dotnet test Ambev.DeveloperEvaluation.sln
```

---

## 🧩 Postman Collection

The repository includes a **Postman Collection** to simplify API testing and validation.  
You can import it directly into Postman using the file located in this repository:

📁 [`sales_collection.json`](./postman/sales_collection.json)

---

## ✅ Summary

This solution demonstrates:

* A clean implementation of **DDD** principles with aggregate invariants.  
* Structured in layers following the **Onion Architecture** principles
* Asynchronous event handling using Rebus (In-Memory) and Serilog for logging domain events.
* Effective caching and cache invalidation with **Redis**.  
* Unit-tested domain logic ensuring consistency and reliability.
