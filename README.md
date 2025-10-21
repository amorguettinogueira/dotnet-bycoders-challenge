# bycoders - Technical Challenge

A full-stack, containerized solution built with .NET 8, ASP.NET Core, PostgreSQL, EF Core (code‑first), Docker Compose, Swagger, and SignalR for real-time notifications. It includes an API, a Web UI, and a background Worker that processes uploaded CNAB files.

## 📄 Notes

- For housekeeping, the original README.md provided by bycoders was moved to [bycoders-provided-content](bycoders-provided-content/README.md).
- Why the solution name `BCP`? Shorthand for `B`ycoders `C`NAB `P`rocessing.

---

## ⚙️ Tech Stack

- Frontend: ASP.NET Core Razor Pages Web App
- Backend: ASP.NET Core Web API
- Worker: .NET BackgroundService that processes files and publishes events
- Database: PostgreSQL
- ORM: Entity Framework Core (Code-First)
- Containerization: Docker + Docker Compose
- Documentation: Swagger (OpenAPI)
- Real-time: SignalR notifications from API to Web UI

---

## 🛠️ Setup Instructions

### 1) Required Tools
Install:
- Visual Studio 2022 (or VS Code with C# extension)
- .NET SDK 8.0
- Docker Desktop 4.48+

Optional:
- pgAdmin (inspect PostgreSQL)
- Postman/HTTPie (test APIs)
- EF Core CLI (for migrations):
  ```
  dotnet tool install --global dotnet-ef
  ```

### 2) Clone the Repository

```
git clone https://github.com/amorguettinogueira/dotnet-bycoders-challenge.git
cd dotnet-bycoders-challenge
```

### 3) Create a .env File
Create a `.env` file at the repo root with:
```
POSTGRES_USER=your_db_user
POSTGRES_PASSWORD=your_db_password
POSTGRES_DB=your_db_name
```
Tips:
- Quote values containing `#`, `$`, `=`, or spaces
- No spaces around `=`
- `#` starts a comment unless escaped

This file is ignored by Git.

### 4) Build and Run
Start Docker first, then:
- Windows: `run.bat`
- Linux/macOS: `./run.sh`

This will:
- Run unit tests
- Build images and start containers (db, api, web, worker)
- Apply EF Core migrations automatically on API startup

---

## 📂 Directory overview
- `bcp.API` — ASP.NET Core API (SignalR hub lives here)
- `bcp.Worker` — background worker service
- `bcp.Web` — Razor Pages UI
- `bcp.Infrastructure` — data access and services
- `bcp.Application` — application-level interfaces and DTOs
- `bcp.Domain` — domain models
- `tests/` — unit test projects for each layer

---

## 🔌 Ports and URLs
- Web UI: [http://localhost:5000](http://localhost:5000)
- API base URL: [http://localhost:5163](http://localhost:5163)
- Swagger UI: [http://localhost:5163/swagger](http://localhost:5163/swagger)
- SignalR hub (used by UI): [http://localhost:5163/hubs/notifications](http://localhost:5163/hubs/notifications)

Note: Docker Compose service names are `api`, `web`, `db`, and `worker`. Volumes share uploads and data-protection keys between containers.

---

## 📘 API Overview
Primary endpoints:
- GET `/api/files` — List uploaded files
- GET `/api/files/{id}/summary` — Store balance aggregation for a file
- POST `/api/files/upload` — Upload a `.txt` CNAB file (multipart form-data)
- POST `/api/notifications/file-processed` — Internal endpoint used by the worker to notify the API, which then broadcasts to the UI via SignalR

Example upload via curl:
```
curl -F "file=@/path/to/file.txt" http://localhost:5163/api/files/upload
```

Real-time updates: When the worker finishes processing a file, the API publishes a `FileProcessed` SignalR event, and the Web UI automatically refreshes.

---

## 🧩 Architecture (High level)
- `db` (PostgreSQL): container with healthcheck and persisted data volume
- `api` (ASP.NET Core Web API): applies EF migrations on startup, exposes REST endpoints and a SignalR hub
- `worker` (.NET BackgroundService): polls for pending files and processes them, then notifies the API
- `web` (Razor Pages): uploads files, lists files, and listens to SignalR notifications from the API

### Diagram
```mermaid
graph TD;
    A[User Browser] -->|Upload .txt| B[Web UI (Razor Pages)];
    B -->|POST multipart| C[API /api/files/upload];
    C -->|Write file| V[(shared-data volume\n/app/uploads)];
    C -->|Insert Pending| D[(PostgreSQL)];
    W[Worker Service] -->|Poll Pending| D;
    W -->|Read file| V;
    W -->|Process + persist| D;
    W -->|POST processed| N[API /api/notifications/file-processed];
    N -->|Broadcast FileProcessed| H[SignalR Hub];
    H -->|Auto-refresh| A;
```

Uploads are shared via a Docker volume mounted at `/app/uploads` for API and Worker. Data protection keys for the Web UI are persisted in a shared volume at `/root/.aspnet/DataProtection-Keys`.

---

## 🧪 Testing
The run scripts execute `dotnet test` on the solution before starting containers. You can also run tests manually:
```
dotnet test Bcp.sln --nologo --verbosity minimal
```

---

## 🗄️ Migrations (manual)
Run from the solution root:
```
cd Bcp.Infrastructure
# Add a new migration (example: InitialCreate)
dotnet ef migrations add InitialCreate --startup-project ../Bcp.API
# Apply migrations to the database
dotnet ef database update --startup-project ../Bcp.API
```

---

## 🧠 Notes
- Secrets/config via `.env` and Docker Compose env vars
- CORS is configured in the API to allow the Web UI origin (container and host)
- The Web UI uses SignalR v8 browser client

---

## 🧑‍💻 Author
Adriano Nogueira — building clean, scalable, and secure full-stack solutions.  
mailto:adriano@nogueira.tech  
[LinkedIn](https://www.linkedin.com/in/amnogueira/)
