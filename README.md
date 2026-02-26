# 📓 Diary App — ASP.NET Core Backend

REST API for a personal diary application with JWT authentication, Entity Framework Core, and SQLite.

---

## Project Structure

```
DiaryApp/
├── Controllers/
│   ├── AuthController.cs      # POST /api/auth/register, /api/auth/login
│   └── PostsController.cs     # CRUD /api/posts
├── Data/
│   └── AppDbContext.cs        # EF Core DbContext
├── DTOs/
│   └── Dtos.cs                # Request/Response models
├── Migrations/
│   └── ...                    # EF Core migrations
├── Models/
│   ├── User.cs
│   └── Post.cs
├── Services/
│   ├── AuthService.cs         # Registration, login, JWT token generation
│   └── PostService.cs         # Post business logic
├── appsettings.json
├── DiaryApp.csproj
└── Program.cs
```

---

## Quick Start

### 1. Install dependencies

```bash
cd DiaryApp
dotnet restore
```

### 2. Run the application

```bash
dotnet run
```

The `diary.db` SQLite database is created automatically on first launch.

### 3. Swagger UI

Open [http://localhost:5000/swagger](http://localhost:5000/swagger) to explore and test the API interactively.

---

## API Endpoints

### 🔐 Authentication

| Method | URL | Description |
|--------|-----|-------------|
| `POST` | `/api/auth/register` | Create a new account |
| `POST` | `/api/auth/login` | Sign in to an existing account |

Both endpoints return a JWT token. All subsequent requests must include the following header:

```
Authorization: Bearer <token>
```

#### Register

```json
POST /api/auth/register
{
  "username": "ivan",
  "email": "ivan@example.com",
  "password": "secret123"
}
```

#### Login

```json
POST /api/auth/login
{
  "email": "ivan@example.com",
  "password": "secret123"
}
```

**Response:**

```json
{
  "token": "eyJhbGci...",
  "username": "ivan",
  "email": "ivan@example.com",
  "expiresAt": "2024-01-08T12:00:00Z"
}
```

---

### 📝 Posts (require authentication)

| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/api/posts?page=1&pageSize=10` | Paginated feed (newest first) |
| `GET` | `/api/posts/{id}` | Get a single post by ID |
| `POST` | `/api/posts` | Create a new post |
| `PUT` | `/api/posts/{id}` | Update an existing post |
| `DELETE` | `/api/posts/{id}` | Delete a post |

#### Create a post

```json
POST /api/posts
{
  "title": "Great day!",
  "content": "I finally finished the project today...",
  "mood": "😊 happy"
}
```

#### Feed response

```json
{
  "items": [
    {
      "id": 3,
      "title": "Another day",
      "content": "...",
      "mood": "😐 neutral",
      "createdAt": "2024-01-07T18:30:00Z",
      "updatedAt": "2024-01-07T18:30:00Z"
    }
  ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5
}
```

---

## Configuration

In `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=diary.db"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS"
  }
}
```

> ⚠️ In production, never store secrets in `appsettings.json`. Use **User Secrets** or environment variables instead:
> ```bash
> dotnet user-secrets set "Jwt:Key" "YourProductionSecret"
> ```

---

## Dependencies

| Package | Purpose |
|---------|---------|
| ASP.NET Core 10 | Web framework |
| Entity Framework Core 10 | ORM |
| SQLite | Database |
| BCrypt.Net-Next | Password hashing |
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT authentication |
| Swashbuckle | Swagger UI |
