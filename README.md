# StudentPortal – PRN232 Lab 1

A complete 3-layer ASP.NET Core 8 Web API demonstrating Repository Pattern,
Service Layer, EF Core Code First, and Swagger/OpenAPI documentation.

---

## Project Structure

```
StudentPortal/
├── StudentPortal.sln
├── StudentPortal.Repositories/          # Data Access Layer
│   ├── Entities/                        # EF Entity models
│   │   ├── Semester.cs
│   │   ├── Course.cs
│   │   ├── Subject.cs
│   │   ├── Student.cs
│   │   ├── CourseSubject.cs             # Many-to-many: Course ↔ Subject
│   │   └── Enrollment.cs               # Many-to-many: Student ↔ Course
│   ├── Configurations/                  # IEntityTypeConfiguration (Fluent API)
│   ├── Context/
│   │   └── ApplicationDbContext.cs
│   ├── Common/
│   │   ├── QueryParameters.cs          # Pagination / filter / sort
│   │   └── PagedResult.cs
│   ├── Interfaces/                      # Repository contracts
│   │   ├── IGenericRepository.cs
│   │   ├── ISemesterRepository.cs
│   │   ├── ICourseRepository.cs
│   │   ├── ISubjectRepository.cs
│   │   └── IStudentRepository.cs
│   └── Implementations/                # Concrete EF repositories
│
├── StudentPortal.Services/              # Business Logic Layer
│   ├── Models/
│   │   └── BusinessModels.cs           # Domain / business models
│   ├── Mappings/
│   │   └── EntityMapper.cs             # Entity ↔ Business model mapping
│   ├── Interfaces/                      # Service contracts
│   └── Implementations/                # Service classes with business rules
│
└── StudentPortal.API/                   # Presentation Layer
    ├── Controllers/
    │   ├── SemestersController.cs
    │   ├── CoursesController.cs
    │   ├── SubjectsController.cs
    │   └── StudentsController.cs
    ├── DTOs/
    │   ├── Request/RequestModels.cs     # Input DTOs (CreateXxx, UpdateXxx, Pagination)
    │   └── Response/ResponseModels.cs  # Output DTOs + ApiResponse<T> envelope
    ├── Extensions/
    │   ├── ServiceCollectionExtensions.cs   # DI wiring
    │   ├── ResponseMapper.cs               # Business model → Response DTO
    │   └── GlobalExceptionMiddleware.cs    # Centralized 500 handling
    ├── Program.cs                          # Bootstrap: DI, EF, Swagger, CORS
    └── appsettings.json
```

---

## Architecture Overview

```
Client
  │
  ▼
[API Layer]  Controllers receive HTTP requests, map Request DTOs → Business Models
  │                validate inputs, return ApiResponse<T> envelopes
  ▼
[Service Layer]  Business logic: uniqueness checks, cross-entity validation,
  │              maps Entity ↔ Business Model via EntityMapper
  ▼
[Repository Layer]  EF Core queries, pagination, sorting, filtering
  │
  ▼
[SQL Server]  Code-First schema via EF migrations
```

### Four Model Types

| Type             | Namespace                          | Purpose                          |
|------------------|------------------------------------|----------------------------------|
| Entity Model     | `StudentPortal.Repositories.Entities` | Maps directly to DB tables       |
| Business Model   | `StudentPortal.Services.Models`       | Carries domain logic in services |
| Request Model    | `StudentPortal.API.DTOs.Request`      | Validates client input           |
| Response Model   | `StudentPortal.API.DTOs.Response`     | Shapes API output; never exposes entities |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server (LocalDB is fine for development — ships with Visual Studio)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## Setup & Run

### 1. Clone / open the solution

```bash
cd StudentPortal
```

### 2. Restore NuGet packages

```bash
dotnet restore
```

### 3. Configure the connection string

Edit `StudentPortal.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentPortalDb_Dev;Trusted_Connection=True;"
  }
}
```

For SQL Server Express:
```
Server=.\\SQLEXPRESS;Database=StudentPortalDb;Trusted_Connection=True;
```

### 4. Create & apply EF migrations

```bash
# From the solution root
dotnet ef migrations add InitialCreate \
  --project StudentPortal.Repositories \
  --startup-project StudentPortal.API \
  --output-dir Migrations

dotnet ef database update \
  --project StudentPortal.Repositories \
  --startup-project StudentPortal.API
```

> **Note:** The API project has `db.Database.Migrate()` in `Program.cs`, so migrations
> also run automatically on first startup.

### 5. Run the API

```bash
dotnet run --project StudentPortal.API
```

Open **http://localhost:5000** → Swagger UI loads automatically.

---

## API Endpoints

### Semesters – `/api/semesters`

| Method | Endpoint              | Description                       |
|--------|-----------------------|-----------------------------------|
| GET    | `/api/semesters`      | List all (paginated, searchable)  |
| GET    | `/api/semesters/{id}` | Get by ID (includes courses)      |
| POST   | `/api/semesters`      | Create new semester               |
| PUT    | `/api/semesters/{id}` | Update existing semester          |
| DELETE | `/api/semesters/{id}` | Delete semester                   |

### Courses – `/api/courses`

| Method | Endpoint            | Description                              |
|--------|---------------------|------------------------------------------|
| GET    | `/api/courses`      | List all (filter by `?semesterId=1`)     |
| GET    | `/api/courses/{id}` | Get by ID (includes subjects)            |
| POST   | `/api/courses`      | Create new course                        |
| PUT    | `/api/courses/{id}` | Update existing course                   |
| DELETE | `/api/courses/{id}` | Delete course                            |

### Subjects – `/api/subjects`

| Method | Endpoint             | Description                       |
|--------|----------------------|-----------------------------------|
| GET    | `/api/subjects`      | List all (paginated, searchable)  |
| GET    | `/api/subjects/{id}` | Get by ID                         |
| POST   | `/api/subjects`      | Create new subject                |
| PUT    | `/api/subjects/{id}` | Update existing subject           |
| DELETE | `/api/subjects/{id}` | Delete subject                    |

### Students – `/api/students`

| Method | Endpoint             | Description                            |
|--------|----------------------|----------------------------------------|
| GET    | `/api/students`      | List all (paginated, searchable)       |
| GET    | `/api/students/{id}` | Get by ID (includes enrollments)       |
| POST   | `/api/students`      | Create new student                     |
| PUT    | `/api/students/{id}` | Update existing student                |
| DELETE | `/api/students/{id}` | Delete student                         |

### Query Parameters (GET collections)

| Parameter       | Type    | Default | Description                            |
|-----------------|---------|---------|----------------------------------------|
| `page`          | int     | 1       | Page number                            |
| `pageSize`      | int     | 10      | Items per page (max 50)                |
| `search`        | string  | —       | Full-text search on name/email/code    |
| `sortBy`        | string  | —       | Field to sort (`name`, `email`, etc.)  |
| `sortDescending`| bool    | false   | Reverse sort order                     |

**Example:**
```
GET /api/students?page=1&pageSize=5&search=nguyen&sortBy=name&sortDescending=false
```

---

## Response Envelope

Every endpoint returns the same shape:

```json
{
  "success": true,
  "message": "Retrieved 3 student(s).",
  "data": { ... },
  "errors": null
}
```

Error example (400):

```json
{
  "success": false,
  "message": "Email 'x@y.com' is already registered.",
  "data": null,
  "errors": ["Email 'x@y.com' is already registered."]
}
```

---

## Sample Requests

### Create a Student

```bash
curl -X POST http://localhost:5000/api/students \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Nguyen Van An",
    "email": "an.nguyen@fpt.edu.vn"
  }'
```

### Create a Course (linked to Semester 1)

```bash
curl -X POST http://localhost:5000/api/courses \
  -H "Content-Type: application/json" \
  -d '{
    "courseName": "PRN232 – ASP.NET Web API Development",
    "semesterId": 1
  }'
```

### Search subjects

```bash
curl "http://localhost:5000/api/subjects?search=PRN&sortBy=code"
```

---

## Business Rules

- `SubjectCode` must be unique across all subjects.
- `Email` must be unique across all students.
- A Course's `SemesterId` must reference an existing Semester.
- `Semester.EndDate` must be after `StartDate` (validated in controller).
- Duplicate enrollments (same Student + Course) are prevented by a unique index.

---

## Seeded Data

On first migration, the following data is inserted automatically:

**Semesters:**
- Spring 2025 (Jan 6 – May 23, 2025)
- Fall 2025 (Aug 18 – Dec 19, 2025)

**Subjects:**
- PRN232 – ASP.NET Web API (3 credits)
- PRJ301 – Java Web Application (3 credits)
- SWD392 – Software Development (3 credits)
