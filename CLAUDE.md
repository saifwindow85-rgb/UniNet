# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build / Run / Migrations

The solution file is the XML-based `UniNet.slnx` (not `.sln`). The Web host project `UniNet/` is the startup project (holds `appsettings.json` + `Program.cs`); `DataAccessLayer/` holds the EF Core `AppDbcontext` and migrations.

```bash
# Build entire solution
dotnet build UniNet.slnx

# Run the API (launches UniNet/ — Swagger UI is enabled only in Development)
dotnet run --project UniNet/UniNet.csproj

# EF Core migrations (DAL project has the context, Web project supplies the connection string/config)
dotnet ef migrations add <Name>  --project DataAccessLayer --startup-project UniNet
dotnet ef database update        --project DataAccessLayer --startup-project UniNet
dotnet ef migrations remove       --project DataAccessLayer --startup-project UniNet   # last migration only
```

No test projects exist. NuGet packages: EF Core 8 + SqlServer, FluentValidation, BCrypt.Net-Next, Swashbuckle, JwtBearer — all `8.x`. Target framework is `net8.0` for every project.

## Architecture

Layered (Clean-ish) solution with **Contracts as a shared DTO library referenced by all layers**, and interface/implementation split across projects:

```
UniNet (Web)  →  Application (Services)  →  DataAccessLayer (Repositories)  →  Domain (Entities)
   ↑                  ↑                         ↑                                ↑
   └──── Contracts (DTOs/Requests/Responses/Enums/Results) ← referenced by all ──┘
```

- **Domain** holds entities **and all interfaces** — service interfaces (`IXxxService`), repository interfaces (`IXxxRepository`), and `IUnitOfWorkRepository` which aggregates every repository as a property. Domain references nothing project-wise; it depends only on Contracts.
- **DataAccessLayer** implements repositories, the `UnitOfWorkRepository` (transaction + `CompleteAsync/Begin/Commit/Rollback`), the `AppDbcontext`, per-entity EF Configurations under `DataAccessLayer/Configurations/`, and migrations.
- **Application** implements services (business logic) and FluentValidation validators. Services receive `IUnitOfWorkRepository` + `IServiceProvider` (used to resolve `IValidator<T>` per request) and return `AddUpdateServiceResponse<T>` result envelopes.
- **UniNet (Web)** hosts controllers, middleware, JWT config, custom authorization handlers, and `Program.cs` DI wiring.

### DI wiring (manual, two extension classes)

`Program.cs` calls three extension methods. When adding a new service/repository/validator you must register it in **all** of these:

- `Application/Extensions/AddServicesToDIContainer.cs` → `ServicesToDI()` (services) and `Validators()` (FluentValidation `IValidator<T>` registrations).
- `DataAccessLayer/Extensions/RepositoriesToDIContainer.cs` → `AddRepoSitoriesToDIContainer()` (repos + `IUnitOfWorkRepository`).
- New repository must also be exposed as a property on `IUnitOfWorkRepository` **and** `UnitOfWorkRepository`.

### Request flow (canonical pattern)

Services return `AddUpdateServiceResponse<T>` (in `Contracts/Responses/`), a result envelope with `IsSuccess`, `Data`, `Errors`, and an `ErrorType` (`EnErrorTypes`). Build responses via the static factories: `.Success(data)`, `.Failure(...)`, `.AlreadyExists<T>()`, `.InvalidRelatedData()`, `.ResourceDoesntExist<T>()`, `.InvalidData`.

Controllers translate the envelope to `ActionResult` via the extension methods in `UniNet/Extensions/ControllersExtensions.cs`:
- `response.ToActionResult<T>()` — maps `EnErrorTypes` → HTTP status (NotFound 404, InvalidData 400, ExistedResource 409, etc.).
- `pagedResult.ToPagedActioneResult<T>()` — paged reads.
- `result.ToDeleteActionResult<TEntity>(id)` — delete → 204/404.
- `resource.GetResourceEndpoints(id, entityName)` — single-resource get → 200/404.

### Authorization (two layers, applied per-controller)

1. **Role-based** via `[Authorize(Roles = "Super Admin,UniversityAdmin,...")]`.
2. **Resource ownership** via ASP.NET Core's `IAuthorizationService` + a custom policy. Pattern (see `CollegeController`): the controller fetches an `XxxAuthorizationInfo` DTO from the service, calls `await _authorizationService.AuthorizeAsync(User, authInfo, "CollegeOwnerPolicy")`, and returns `AuthorizationFailed()` on failure. The policy is `OwnershipRequirement`; handler is `CollegeOwnerHandler` (`UniNet/Authorization/`). The Employee feature's equivalent handler (`Authorization/AuthorizationHandlers/EmployeeHandlers/`) is scaffolded but not yet implemented.

### Auth / Identity (custom, not ASP.NET Identity)

- `User`/`Role`/`UserRole`/`RefreshToken` are own entities (no Identity library). Passwords hashed with BCrypt.
- JWT issued by `UniNet/Helpers/JwtTokenFactory.cs`; options bound from `Configuration["Jwt"]` into `JWTOption` (`DataAccessLayer/Configurations/Options/`). Custom `OnChallenge`/`OnForbidden` events write JSON 401/403 bodies.
- **Custom claims** carry scope: `CustomClaimTypes` (`UniNet/CustomClaims/`) define `UniversityId`, `CollegeId`, `DepartmentId`, `BatchId`. `JwtTokenFactory` emits them when present; `ICurrentUserService` (`Application/Services/IdentityServices/CurrentUserService.cs`) reads them back, exposing `UserId`, `UserName`, and the nullable scope IDs.
- The `ToEmployeeScope()` extension on `ICurrentUserService` builds an `EmployeeScope` from the current user's scope claims — used to filter Employee listings.

### Mapping (manual, no AutoMapper)

Repositories define `ToDTO` / `ToInfo` as `Expression<Func<TEntity, DTO>>` projection expressions and apply them via `Select(...)` in EF queries (see `EmployeeRepository`). Services construct/update entities manually. There is no MediatR, no AutoMapper.

### BaseEntity audit

Most entities derive from `BaseEntity` (`CreatedAt`, `UpdatedAt`, `CreatedByUserId`, `UpdatedByUserId`). `AppDbcontext.ConfigureBaseEntity` wires the self-referencing `CreatedByUser`/`UpdatedByUser` with `Restrict` delete behavior — new entities get audit fields from `ICurrentUserService.UserId` in the service layer.

## Conventions

- Solution folders mirror layer projects: `Contracts/Requests/<Area>Requests/`, `Contracts/Responses/<Area>Response/`, `Application/Services/<Area>Service/`, `Application/Validators/<Area>Validator/`, `DataAccessLayer/Repos/<Area>Repository/`, `DataAccessLayer/Configurations/<Area>/`, `Domain/Entities/<Area>/`, `Domain/Interfaces/<Area>Interfaces/`. Follow this grouping when adding a new domain area.
- Routes are `[Route("api/[controller]")]` with named endpoints (`[HttpGet(Name = "...")]`). Query params use `[FromQuery]` parameter objects (e.g. `CollegeIdParameter`, `PagedResultParameters` in `Contracts/Requests/RequestParameters/`).
- The default remote branch is `master` (there is no `main`). Feature work happens on per-area branches (`CollegeBranch`, `EmployeeBranch`, `UserBranchV2`, etc.). When diffing current work, compare against `master`.

## Work in progress (as of EmployeeBranch)

The Employee feature is **complete**. All admin types are implemented (`AddUniversityAdmin`/`UpdateUniversityAdmin`, `AddCollegeAdmin`/`UpdateCollegeAdmin`, `AddDepartmentAdmin`/`UpdateDepartmentAdmin`). Scope-based access control (`IsWithinScope`) is shared between the service layer and the `EmployeeOwnerHandler` via `Contracts/Common/Extensions/EmployeeScopeExtension.cs`. The `GetEmployeeById` endpoint uses `EmployeeOwnerPolicy` + `IAuthorizationService` following the `CollegeController` pattern. A `TestDataSeeder` exists under `DataAccessLayer/Seeds/` for development seeding (hundreds of rows, idempotent, BCrypt-hashed passwords).

## Next planned work (not yet started)

- **Logging**: Add `ILogger` to middleware and services.
- **Unit Testing**: Create a test project (xUnit) with tests for validators and services.
- **Frontend SPA**: Static files under `UniNet/wwwroot/` (currently scaffolded but needs runtime validation and fixes).