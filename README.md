# 📓 Diary App — ASP.NET Core 8 Backend

REST API для личного дневника с JWT-аутентификацией, Entity Framework Core и SQLite.

---

## Структура проекта

```
DiaryApp/
├── Controllers/
│   ├── AuthController.cs      # POST /api/auth/register, /api/auth/login
│   └── PostsController.cs     # CRUD /api/posts
├── Data/
│   └── AppDbContext.cs        # EF Core DbContext
├── DTOs/
│   └── Dtos.cs                # Request/Response модели
├── Migrations/
│   └── ...                    # EF Core миграции
├── Models/
│   ├── User.cs
│   └── Post.cs
├── Services/
│   ├── AuthService.cs         # Регистрация, логин, JWT
│   └── PostService.cs         # Бизнес-логика постов
├── appsettings.json
├── DiaryApp.csproj
└── Program.cs
```

---

## Быстрый старт

### 1. Установка зависимостей

```bash
cd DiaryApp
dotnet restore
```

### 2. Запуск

```bash
dotnet run
```

База данных `diary.db` (SQLite) создаётся автоматически при первом запуске.

### 3. Swagger UI

Откройте [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## API Endpoints

### 🔐 Аутентификация

| Метод | URL | Описание |
|-------|-----|----------|
| `POST` | `/api/auth/register` | Регистрация |
| `POST` | `/api/auth/login` | Вход |

Оба endpoint'а возвращают JWT-токен. Все последующие запросы требуют заголовок:
```
Authorization: Bearer <token>
```

#### Регистрация
```json
POST /api/auth/register
{
  "username": "ivan",
  "email": "ivan@example.com",
  "password": "secret123"
}
```

#### Логин
```json
POST /api/auth/login
{
  "email": "ivan@example.com",
  "password": "secret123"
}
```

**Ответ:**
```json
{
  "token": "eyJhbGci...",
  "username": "ivan",
  "email": "ivan@example.com",
  "expiresAt": "2024-01-08T12:00:00Z"
}
```

---

### 📝 Посты (требуют авторизации)

| Метод | URL | Описание |
|-------|-----|----------|
| `GET` | `/api/posts?page=1&pageSize=10` | Лента (новые → старые) |
| `GET` | `/api/posts/{id}` | Получить пост по ID |
| `POST` | `/api/posts` | Создать пост |
| `PUT` | `/api/posts/{id}` | Обновить пост |
| `DELETE` | `/api/posts/{id}` | Удалить пост |

#### Создание поста
```json
POST /api/posts
{
  "title": "Отличный день!",
  "content": "Сегодня я наконец завершил проект...",
  "mood": "😊 happy"
}
```

#### Лента постов (ответ)
```json
{
  "items": [
    {
      "id": 3,
      "title": "Ещё один день",
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

## Конфигурация

В `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=diary.db"
  },
  "Jwt": {
    "Key": "ВАШ_СЕКРЕТНЫЙ_КЛЮЧ_МИНИМУМ_32_СИМВОЛА"
  }
}
```

> ⚠️ В продакшене используйте **User Secrets** или переменные среды:
> ```bash
> dotnet user-secrets set "Jwt:Key" "YourProductionSecret"
> ```

---

## Используемые технологии

| Библиотека | Назначение |
|------------|-----------|
| ASP.NET Core 8 | Web framework |
| Entity Framework Core 8 | ORM |
| SQLite | База данных |
| BCrypt.Net-Next | Хеширование паролей |
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT |
| Swashbuckle | Swagger UI |
