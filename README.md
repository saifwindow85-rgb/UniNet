# UniNet

A **university management system** built with ASP.NET Core 8. It manages the full academic hierarchy — universities, colleges, departments, batches, and sections — along with employees, students, study programs, and content.

> **Status:** Backend APIs are functional. Core modules (Academic Structure, Employees, Identity) are complete. Student management, study results, and the frontend SPA are in active development.

---

## Tech Stack

- **.NET 8** — ASP.NET Core Web API
- **Entity Framework Core 8** + SQL Server
- **JWT Bearer Authentication** — custom implementation (not ASP.NET Identity)
- **BCrypt.Net-Next** — password hashing
- **FluentValidation** — input validation
- **Swashbuckle** — Swagger / OpenAPI docs (Development only)

---

## Architecture

Clean-ish layered architecture with **Contracts** as a shared DTO library:

```
UniNet (Web API)
  → Application (Services + Validators)
  → DataAccessLayer (EF Core + Repositories + Migrations)
  → Domain (Entities + Interfaces)

Contracts (DTOs / Requests / Responses / Enums)
  ↑ referenced by all layers
```

Key patterns used:
- **Repository + Unit of Work** — per-entity repositories aggregated under `IUnitOfWorkRepository`
- **Result Envelopes** — `AddUpdateServiceResponse<T>` with static factories (`Success`, `Failure`, `AlreadyExists`, ...)
- **Manual Mapping** — `Expression<Func<TEntity, TDto>>` projections in repositories (no AutoMapper)
- **Resource-Based Authorization** — custom `OwnershipRequirement` + handlers (`CollegeOwnerHandler`, `EmployeeOwnerHandler`)
- **Custom JWT Claims** — `UniversityId`, `CollegeId`, `DepartmentId`, `BatchId` for scope-based access

---

## Modules

| Area | Status | Description |
|---|---|---|
| **Academic Structure** | ✅ Complete | Universities, Colleges, Departments, Batches, Sections — full CRUD with ownership checks |
| **Identity & Auth** | ✅ Complete | Custom JWT login/logout/refresh, Users, Roles, UserRoles, BCrypt passwords |
| **Employees** | ✅ Complete | UniversityAdmin / CollegeAdmin / DepartmentAdmin — scope-based CRUD |
| **Student Status** | 🔄 In Progress | Lookup statuses (Enrolled, Graduated, Suspended) |
| **Students** | 🔄 In Progress | Student enrollment with Batch/Section assignment |
| **Study** | 📋 Planned | Subjects, Semesters, SectionSubject assignments |
| **Student Results** | 📋 Planned | Grades (Midterm / Practical / Final / Total) |
| **Content** | 📋 Planned | Posts and Announcements with image attachments |
| **Frontend** | 🔄 In Progress | Static SPA in `wwwroot/` (HTML/CSS/JS), served by the API |

---

## Authentication

Custom JWT implementation (no ASP.NET Identity):

- `POST /api/Login/login` — returns `AccesseToken` + `RefreshToken`
- `POST /api/Login/refresh` — rotates tokens
- `POST /api/Login/logOut` — revokes refresh token
- Claims carry **scope** (`UniversityId`, `CollegeId`, `DepartmentId`, `BatchId`) for hierarchical access control
- Passwords hashed with **BCrypt**

---

## Build & Run

```bash
# Build
dotnet build UniNet.slnx

# Run (Swagger UI enabled in Development)
dotnet run --project UniNet/UniNet.csproj

# Migrations (DAL has the context; Web supplies config)
dotnet ef migrations add <Name> --project DataAccessLayer --startup-project UniNet
dotnet ef database update        --project DataAccessLayer --startup-project UniNet
```

---

## Development Seeding

`TestDataSeeder` (in `DataAccessLayer/Seeds/`) populates realistic, interrelated test data on startup:
- 3 universities, 9 colleges, 27 departments, 27 batches, 54 sections
- ~39 admin employees (Univ/College/Dept levels)
- ~216 students
- Subjects, semesters, content, and images

All seeded accounts use password `P@ssw0rd123!` (hashed with real BCrypt).

---

## What I Learned / Practiced

- Designing a **layered backend** with clear separation of concerns
- Implementing **custom JWT authentication** with refresh-token rotation
- Building **resource-based authorization** with ASP.NET Core policy handlers
- Using **FluentValidation** for centralized input validation
- Writing **idempotent database seeders** with realistic relational data
- Managing **EF Core migrations** and SQL Server configurations
- Working with **manual DTO projections** and result envelopes

---

## Next Steps

- Complete **Student** enrollment and **Student Results** modules
- Add **ILogger** across middleware and services
- Create **xUnit test project** for validators and services
- Stabilize and ship the **frontend SPA**

---

*Built as a learning project to practice Clean-ish architecture, custom auth, and full-stack API design.*