# FCG.Users.API

Microsserviço de cadastro e autenticação JWT.

## Variáveis de ambiente / appsettings

| Chave | Descrição |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL (`fcg_users`) |
| `RabbitMq:*` | Broker para publicar `UserCreatedEvent` |
| `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` | Token compartilhado com Catalog |

## Executar

```bash
dotnet ef database update --project src/FCG.Users.Infrastructure --startup-project src/FCG.Users.API
dotnet run --project src/FCG.Users.API
```

Swagger: http://localhost:5001/swagger

Admin seed: `admin@admin.com` / `Teste@123`
