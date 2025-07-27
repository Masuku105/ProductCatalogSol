# ProductCatalog Solution

## 🚀 Overview

This `.NET Core` solution includes:
- **Web API** project for product catalog operations
- **Web UI** (MVCfront-end consuming the API)
- **Unit & integration tests** (xUnit, mocks)

---

## 🖥️ Prerequisites

- .NET Core SDK (3.1)
- Redis server (Ensure you change the url on the api appsettings)
- Redis via Memurai/Standalone install

---

## 🛠️ Running Locally

1. Clone the repo 
2. Restore and build:

   ```bash
   dotnet restore
   dotnet build

   
✅ SOLID / OOP & Design Patterns
Dependency Injection throughout (IProductService, IProductRepository)

Single Responsibility Principle: Controllers only handle request flow; service logic is in service layer.

Open‑Closed & Dependency Inversion: Interfaces allow substituting mocks or new implementations.

Repository Pattern encapsulates API calls, hiding HTTP logic from higher layers.

This solution showcases:

Clean, well-layered architecture

Good use of SOLID principles, OOP, and standard design patterns

Efficient caching strategy, leveraging decorator/proxy and read-through implementations

Full local development setup and test coverage

