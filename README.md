# Token Revocation Service

A centralized token revocation service built with **ASP.NET Core Web API** to manage and invalidate authentication tokens across distributed applications and microservices.

The service provides a mechanism to revoke JWTs before their natural expiration time, helping applications prevent unauthorized access when users log out, sessions are terminated, or tokens are compromised.

## Features

- **Token Revocation:** Revoke authentication tokens before their expiration.
- **Expiration-Based Revocation:** Maintain revoked tokens until their original expiration time.
- **Centralized Revocation Management:** Allow multiple services to share token revocation information.
- **RESTful API:** Expose token revocation functionality through HTTP endpoints.
- **Asynchronous Operations:** Support asynchronous token storage and revocation operations.
- **Cancellation Support:** Handle cancellation tokens for asynchronous operations.
- **Storage Abstraction:** Separate token revocation business logic from the underlying storage implementation.
- **PostgreSQL Integration:** Support persistent token revocation storage.
- **JWT Integration:** Designed to work alongside JWT-based authentication systems.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Programming language |
| ASP.NET Core Web API | REST API development |
| Entity Framework Core | Database access |
| PostgreSQL | Persistent storage |
| Redis | Optional distributed caching/storage |
| JWT | Token-based authentication |
| Dependency Injection | Service composition |
| Swagger / OpenAPI | API documentation |

## Architecture

The service follows a separation-of-concerns approach, allowing the revocation logic to operate independently of the storage implementation.

```text
             Client Application
                     |
                     v
             Authentication API
                     |
                     v
                JWT Token
                     |
                     v
             Protected Services
                     |
                     v
              Token Validation
                     |
                     v
             Revocation Check
                     |
              +------+------+
              |             |
           Revoked       Not Revoked
              |             |
              v             v
          Reject         Continue
          Request        Request
```

### Component Responsibilities

- **API Layer:** Exposes HTTP endpoints for token revocation.
- **Application Layer:** Coordinates token revocation operations.
- **Storage Layer:** Persists revoked tokens and their expiration information.
- **Domain Layer:** Contains core token revocation models and business rules.

## How Token Revocation Works

JWTs are generally stateless. Once issued, a token can remain valid until its expiration time unless the application implements an additional revocation mechanism.

This service addresses that limitation through centralized revocation storage.

1. A client authenticates and receives an access token.
2. The client sends the token when accessing protected resources.
3. When a token needs to be invalidated, the calling application submits a revocation request.
4. The service stores the token and its expiration time.
5. Protected services check the revocation status before accepting the token.
6. Revoked tokens are rejected even if their JWT expiration time has not been reached.
7. Expired revocation records can be removed safely.

## API Reference

### Revoke Token

Revokes a token until its specified expiration time.

**Endpoint**

```http
POST /api/tokens/revoke
```

**Request Body**

```json
{
  "token": "JWT_ACCESS_TOKEN",
  "expiryUtc": "2026-11-10T18:00:00Z"
}
```

**Expected Behavior**

- Validates the incoming token value.
- Uses the supplied expiration timestamp.
- Stores the token revocation information.
- Prevents subsequent acceptance of the revoked token when revocation checks are integrated into protected services.

**Response**

```http
200 OK
```

### Additional Operations

The exact availability of token lookup, cleanup, and other management endpoints depends on the implementation.

## Getting Started

### Prerequisites

- .NET SDK compatible with the project.
- PostgreSQL (if using the PostgreSQL storage implementation).
- Redis (if using the Redis storage implementation).
- Git.
- Visual Studio or Visual Studio Code.

### 1. Clone the Repository

```bash
git clone https://github.com/ziasam/TokenRevocationService.git

cd TokenRevocationService
```

### 2. Configure the Database

Configure the PostgreSQL connection string in `appsettings.json`.

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=tokenrevocation;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### 3. Configure Application Settings

Configure the required application settings and storage provider according to the project configuration.

Keep database credentials and other sensitive values outside source control.

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Apply Database Migrations

If the project uses Entity Framework Core migrations:

```bash
dotnet ef database update
```

### 6. Run the Application

```bash
dotnet run
```

Access the Swagger UI using the URL configured by the application.

## Integration with Authentication Services

The Token Revocation Service can be integrated with a centralized Authentication Service and other protected microservices.

A typical integration flow:

1. The Authentication Service issues JWT access tokens.
2. A client uses the access token to communicate with downstream services.
3. When the client logs out or a token is compromised, the Authentication Service requests token revocation.
4. The Token Revocation Service records the revoked token.
5. Downstream services check the revocation status during authentication.
6. Requests containing revoked tokens are rejected.

## Storage Considerations

Token revocation records must remain available across application instances.

**PostgreSQL**
- Provides persistent storage.
- Supports transactional operations.
- Allows centralized revocation records to be shared across multiple services.
- Requires database access for revocation lookups unless caching is introduced.

**Redis**
- Provides fast lookup operations.
- Supports expiration-based key management.
- Works well for distributed applications with frequent revocation checks.
- Requires appropriate persistence and availability configuration.

The storage implementation should be selected based on consistency, availability, performance, and operational requirements.

## Security Considerations

- Never log complete JWT values.
- Restrict access to revocation endpoints.
- Validate token expiration timestamps.
- Use HTTPS for service communication.
- Protect database credentials.
- Ensure all protected services perform revocation checks.
- Avoid accepting untrusted revocation requests.
- Consider using a token hash as the storage key instead of storing raw JWT values.

## License

Refer to the repository's license for usage and distribution terms.
