# Nigerian Primary School Management System
## Authentication & Authorization Module — Sprint 1

### Solution Architecture (Clean Architecture)

```
NigerianPrimarySchool.sln
└── src/
    ├── Domain/                  ← Enterprise rules, zero dependencies
    │   ├── Common/
    │   │   └── BaseAuditableEntity.cs
    │   ├── Constants/
    │   │   └── Roles.cs         ← Single source of truth for ALL roles & policies
    │   └── NigerianPrimarySchool.Domain.csproj
    │
    ├── Application/             ← Use cases, interfaces — no framework deps
    │   ├── Common/Models/
    │   │   └── Result.cs        ← Discriminated union result type
    │   ├── DTOs/Auth/
    │   │   ├── LoginDto.cs
    │   │   ├── RegisterUserDto.cs
    │   │   └── UserDtos.cs      ← UserProfileDto, ChangePasswordDto, UpdateProfileDto, UserListItemDto
    │   ├── Interfaces/
    │   │   ├── IIdentityService.cs
    │   │   └── ICurrentUserService.cs
    │   └── NigerianPrimarySchool.Application.csproj
    │
    ├── Infrastructure/          ← EF Core, Identity, SQL Server
    │   ├── Identity/
    │   │   ├── ApplicationUser.cs       ← Extended IdentityUser
    │   │   ├── ApplicationRole.cs       ← Extended IdentityRole
    │   │   └── IdentityService.cs       ← IIdentityService implementation
    │   ├── Persistence/
    │   │   ├── ApplicationDbContext.cs
    │   │   ├── DatabaseSeeder.cs        ← Auto-seeds roles + SuperAdmin on first run
    │   │   └── Configurations/
    │   │       └── ApplicationUserConfiguration.cs
    │   ├── DependencyInjection.cs
    │   └── NigerianPrimarySchool.Infrastructure.csproj
    │
    ├── Web/                     ← Blazor Auto server-side host (PWA)
    │   ├── Components/
    │   │   ├── App.razor
    │   │   ├── Routes.razor     ← AuthorizeRouteView wired up
    │   │   ├── _Imports.razor
    │   │   ├── Account/
    │   │   │   ├── Pages/
    │   │   │   │   ├── Login.razor
    │   │   │   │   ├── Logout.razor
    │   │   │   │   ├── ForgotPassword.razor
    │   │   │   │   ├── ResetPassword.razor
    │   │   │   │   ├── AccessDenied.razor
    │   │   │   │   └── Manage/Index.razor   ← Full profile + password change
    │   │   │   ├── Shared/
    │   │   │   │   ├── StatusMessage.razor
    │   │   │   │   ├── RedirectToLogin.razor
    │   │   │   │   └── AccessDenied.razor
    │   │   │   ├── IdentityRedirectManager.cs
    │   │   │   ├── IdentityUserAccessor.cs
    │   │   │   ├── PersistingRevalidatingAuthenticationStateProvider.cs
    │   │   │   └── IdentityComponentsEndpointRouteBuilderExtensions.cs
    │   │   ├── Layout/
    │   │   │   ├── MainLayout.razor     ← RadzenLayout with sidebar + header
    │   │   │   ├── NavMenu.razor        ← Role-gated navigation
    │   │   │   └── EmptyLayout.razor    ← Used by auth pages
    │   │   └── Pages/
    │   │       ├── Dashboard.razor      ← Role-aware dashboard
    │   │       └── Admin/
    │   │           ├── UserManagement.razor
    │   │           ├── AddUserDialog.razor
    │   │           └── EditUserDialog.razor
    │   ├── Identity/
    │   │   └── CurrentUserService.cs
    │   ├── Program.cs           ← Full DI wiring, policy registration, seeding
    │   ├── appsettings.json
    │   └── wwwroot/
    │       ├── css/app.css      ← Nigerian green+gold theme
    │       ├── manifest.json    ← PWA manifest
    │       └── service-worker.js
    │
    └── Web.Client/              ← Blazor WASM client (Auto render mode)
        ├── PersistentAuthenticationStateProvider.cs
        ├── Program.cs
        └── _Imports.razor
```

---

## User Roles

| Role | Description |
|------|-------------|
| `SuperAdmin` | Full unrestricted access |
| `HeadTeacher` | Academic & administrative oversight |
| `SubjectTeacher` | Assigned subjects across classes |
| `ClassTeacher` | Responsible for a specific class/arm |
| `SchoolAccountant` | Fee management & financial records |
| `SchoolBursar` | Bursary operations (same as Accountant) |
| `SchoolBookShopKeeper` | Bookshop stock & sales |
| `SchoolStoreKeeper` | General store, supplies, equipment |
| `Parent` | View own children's records |
| `Student` | Read-only access to own records |

## Authorization Policies

| Policy | Grants access to |
|--------|-----------------|
| `RequireSuperAdmin` | SuperAdmin only |
| `RequireHeadTeacherOrAbove` | SuperAdmin + HeadTeacher |
| `RequireAnyStaff` | All 8 staff roles |
| `RequireTeacher` | HeadTeacher + SubjectTeacher + ClassTeacher |
| `RequireFinance` | SuperAdmin + HeadTeacher + Accountant + Bursar |
| `RequireStore` | SuperAdmin + BookShopKeeper + StoreKeeper |
| `RequireParent` | Parent only |
| `RequireStudent` | Student only |
| `RequireParentOrStudent` | Parent + Student |

---

## Getting Started

### 1. Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB, Express, or full)
- Visual Studio 2022 v17.8+ or VS Code with C# Dev Kit

### 2. Update connection string
Edit `src/Web/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=NigerianPrimarySchoolDb;Trusted_Connection=True;"
}
```

### 3. Add EF Core migration & update database
```bash
cd src/Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Web
dotnet ef database update --startup-project ../Web
```

### 4. Run
```bash
cd src/Web
dotnet run
```
The app seeds **all roles** and a default **SuperAdmin** automatically on first run.

> **Default SuperAdmin credentials** (CHANGE IMMEDIATELY):
> - Email: `superadmin@school.edu.ng`
> - Password: `Admin@1234!`

---

## What was intentionally left for future sprints
- Email sender (`IEmailSender`) — ForgotPassword page generates the token but doesn't send email yet
- Student & parent-specific portals (result views, attendance views)
- Academic, Finance, Store, Bookshop feature modules
- School logo images (`/wwwroot/images/`) — add `school-logo.png`, `icon-192.png`, `icon-512.png`
