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


🚀 How to run the project
This project uses Visual Studio with multiple startup projects configured so that both the API and the Web UI (MVC or Blazor) start together when you press F5 (Start Debugging).

✅ Setting It Up in Visual Studio
In Solution Explorer, right-click the Solution node (top).
Select "Properties", and go to Common Properties → Startup Project.
Choose "Multiple startup projects".
For both the API and Web UI projects, set Action = Start (or Start without debugging if preferred).
Once configured, pressing F5 launches both projects concurrently—so you can hit breakpoints and debug across both the Web UI and API.

