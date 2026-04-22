# BookYourStay Code Patterns & Conventions

This document outlines the coding patterns, architectural principles, and conventions used in the BookYourStay project. All AI assistants and developers must follow these patterns to maintain consistency and code quality.

## 🏗️ Architecture Overview

BookYourStay follows **Clean Architecture** with **CQRS (Command Query Responsibility Segregation)** pattern.

### Backend Structure
```
apps/backend/
├── Features/                    # Feature-based organization
│   └── Auth/                   # Each feature in its own folder
│       ├── AuthController.cs   # HTTP endpoints
│       ├── Commands/          # Write operations
│       ├── Contracts/         # DTOs and responses
│       ├── Domain/            # Business models
│       ├── Extensions/        # Extension methods
│       ├── Options/           # Configuration classes
│       ├── Persistence/       # Data access layer
│       ├── Queries/           # Read operations
│       └── Services/          # Business logic services
├── Shared/                     # Cross-cutting concerns
│   ├── Contracts/             # Shared DTOs
│   ├── Data/                  # Database migrations
│   └── Results/               # Result patterns
└── tests/                     # Test projects
```

## 📋 Core Principles

### 1. Single Responsibility Principle (SRP)
- **One file = One responsibility**
- **One class = One reason to change**
- **One method = One action**

### 2. CQRS Pattern
- **Commands**: Write operations (create, update, delete)
- **Queries**: Read operations (get data)
- **Separation**: Commands and queries never mix

### 3. Clean Architecture
- **Dependency Rule**: Inner layers don't depend on outer layers
- **Domain First**: Business logic is independent of frameworks
- **Dependency Injection**: All dependencies are injected

## 📁 File Organization Patterns

### Feature Structure
Each feature follows this exact structure:
```
Features/{FeatureName}/
├── {FeatureName}Controller.cs    # HTTP endpoints only
├── Commands/                     # Write operations
│   ├── {CommandName}/
│   │   ├── {CommandName}Request.cs
│   │   └── {CommandName}CommandHandler.cs
├── Contracts/                    # Feature-specific DTOs
├── Domain/                       # Business entities
├── Extensions/                   # Extension methods
├── Options/                      # Configuration classes
├── Persistence/                  # Data access
├── Queries/                      # Read operations
│   └── {QueryName}/
│       ├── {QueryName}Query.cs
│       └── {QueryName}QueryHandler.cs
└── Services/                     # Business logic
```

### File Naming Conventions
- **Controllers**: `{FeatureName}Controller.cs`
- **Commands**: `{ActionName}Request.cs`, `{ActionName}CommandHandler.cs`
- **Queries**: `{QueryName}Query.cs`, `{QueryName}QueryHandler.cs`
- **Services**: `{ServiceName}Service.cs`
- **Repositories**: `{FeatureName}Repository.cs`
- **Extensions**: `{FeatureName}Extensions.cs`
- **Options**: `{ServiceName}Options.cs`

## 🔧 Code Patterns

### 1. Request/Response DTOs
```csharp
// Request DTOs - Records for immutability
public sealed record RegisterRequest(
    string FullName,
    string Email,
    string Password) : IRequest<ApplicationResult<AuthResponse>>;

// Response DTOs - Records for immutability
public sealed record AuthResponse(
    Guid SessionId);
```

### 2. Command Handlers
```csharp
public sealed class RegisterCommandHandler(
    AuthRepository repository,
    PasswordService passwordService,
    JwtService jwtService,
    TimeProvider timeProvider)
    : IRequestHandler<RegisterRequest, ApplicationResult<AuthResponse>>
{
    public async Task<ApplicationResult<AuthResponse>> Handle(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        // Business logic here
        return ApplicationResult<AuthResponse>.Ok(response, "Success message");
    }
}
```

### 3. Query Handlers
```csharp
public sealed class GetCurrentUserQueryHandler(
    AuthRepository repository,
    JwtService jwtService,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetCurrentUserRequest, ApplicationResult<CurrentUserResponse>>
{
    public async Task<ApplicationResult<CurrentUserResponse>> Handle(
        GetCurrentUserRequest request,
        CancellationToken cancellationToken)
    {
        // Read logic here
        return ApplicationResult<CurrentUserResponse>.Ok(response, "Retrieved");
    }
}
```

### 4. Domain Models
```csharp
// Use classes for entities that may be mutated
public sealed class User
{
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; set; }
}
```

### 5. Services
```csharp
// Services with dependencies injected
public sealed class JwtService
{
    private readonly JwtOptions _options;
    private readonly TimeProvider _timeProvider;

    public JwtService(IOptions<JwtOptions> options, TimeProvider timeProvider)
    {
        _options = options.Value;
        _timeProvider = timeProvider;
    }

    // Methods here
}
```

### 6. Repositories
```csharp
public sealed class AuthRepository(IDbConnection connection)
{
    private IDbConnection Connection => connection;

    public async Task<User?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM users WHERE email = @Email";
        return await Connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken));
    }
}
```

### 7. Controllers
```csharp
[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return ToResult(result, setCookies: true);
    }

    private IActionResult ToResult<T>(
        ApplicationResult<T> result,
        bool setCookies = false)
    {
        // Response handling logic
    }
}
```

## 🧪 Testing Patterns

### Unit Tests
```csharp
public sealed class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var handler = new RegisterCommandHandler(...);
        var request = new RegisterRequest("John Doe", "john@example.com", "password123");

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}
```

### Integration Tests
```csharp
public sealed class AuthEndpointsTests(BackendWebApplicationFactory factory)
    : IClassFixture<BackendWebApplicationFactory>
{
    [Fact]
    public async Task Register_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## 🔒 Security Patterns

### Cookie Authentication
- Use HttpOnly, Secure, SameSite cookies
- Encrypt sensitive cookie data
- Never expose tokens in API responses

### Password Security
- Use ASP.NET Core Identity PasswordHasher
- Hash refresh tokens with SHA256
- Validate passwords on both client and server

### JWT Tokens
- Short-lived access tokens (15-30 minutes)
- Long-lived refresh tokens (7 days)
- Validate tokens on every request

## 📊 Result Patterns

### ApplicationResult Pattern
```csharp
// Success
return ApplicationResult<AuthResponse>.Ok(response, "Registration successful");

// Bad Request
return ApplicationResult<AuthResponse>.BadRequest("Validation failed", errors);

// Unauthorized
return ApplicationResult<AuthResponse>.Unauthorized("Invalid credentials", errors);

// Not Found
return ApplicationResult<AuthResponse>.NotFound("User not found");
```

## 🗂️ Dependency Injection

### Service Registration
```csharp
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddSingleton<CookieProtector>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));
```

### Lifetime Guidelines
- **Singleton**: Stateless services, configuration, utilities
- **Scoped**: Per-request services, repositories, DbContext
- **Transient**: Services that need fresh instances

## 🛠️ Development Workflow

### 1. Adding a New Feature
1. Create `Features/{FeatureName}/` folder
2. Implement domain models in `Domain/`
3. Create repository in `Persistence/`
4. Add services in `Services/`
5. Implement commands/queries
6. Add controller endpoints
7. Write tests

### 2. Adding a New Command
1. Create `Commands/{CommandName}/` folder
2. Add `{CommandName}Request.cs` record
3. Add `{CommandName}CommandHandler.cs` class
4. Register handler in DI (automatic with MediatR)
5. Add controller endpoint
6. Write unit and integration tests

### 3. Adding a New Query
1. Create `Queries/{QueryName}/` folder
2. Add `{QueryName}Query.cs` record
3. Add `{QueryName}QueryHandler.cs` class
4. Add controller endpoint
5. Write tests

## 📝 Code Style Guidelines

### C# Conventions
- Use `sealed` classes where possible
- Prefer `record` for immutable DTOs
- Use primary constructors
- Use `var` when type is obvious
- Use meaningful variable names
- Add XML documentation for public APIs

### SQL Conventions
- Use parameterized queries
- Use descriptive SQL variable names (@Email, @UserId)
- Use CTEs for complex queries
- Include cancellation tokens

### Error Handling
- Use `ApplicationResult<T>` for business logic errors
- Throw exceptions only for unexpected errors
- Validate input at API boundaries
- Log errors appropriately

## 🔍 Quality Checks

Before committing code:
- ✅ All tests pass
- ✅ Code compiles without warnings
- ✅ Follows established patterns
- ✅ Single responsibility principle maintained
- ✅ Dependencies properly injected
- ✅ Security best practices followed
- ✅ Database migrations tested

## 📚 References

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)

---

**Remember**: When in doubt, look at existing code in the same feature for patterns to follow. Consistency is key to maintainable code.</content>
<parameter name="filePath">/Users/portal/programming/personal/bookyourstay/code.md