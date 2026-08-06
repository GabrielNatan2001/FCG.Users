# FCG.Users

Microsserviço responsável pelo **cadastro de usuários** e **autenticação JWT**. Após criar um usuário, publica o evento `UserCreatedEvent` no RabbitMQ para a **Azure Function** de notificações. Expõe métricas Prometheus em `/metrics`.

## Projetos

| Projeto | Descrição |
|---|---|
| `FCG.Users.API` | API HTTP — registro, login e emissão de token JWT |
| `FCG.Users.Application` | Casos de uso e contratos de mensageria |
| `FCG.Users.Infrastructure` | EF Core (PostgreSQL), RabbitMQ e JWT |
| `FCG.Users.Domain` | Entidades e regras de domínio |

## Imagem Docker

`gabrielnatan2001/fcg-api-users:latest`

## Variáveis de ambiente

| Variável (Docker/K8s) | Obrigatória | Descrição |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Sim | PostgreSQL (`fcg_users`) |
| `MessageBusConfigs__Host` | Sim | RabbitMQ |
| `Publishers__UserCreated__Exchange` | Sim | `fcg.user.created` |
| `Publishers__UserCreated__RoutingKey` | Sim | `notifications.user-created` |
| `Jwt__Key` / `Jwt__Issuer` / `Jwt__Audience` | Sim | JWT (issuer `FCG.Users.API` usado pelo Kong) |

## Executar localmente

```bash
dotnet run --project src/FCG.Users.API
```

- Admin seed: `admin@admin.com` / `Teste@123`
- Via Gateway: `http://localhost:8000/users/api/Auth/login`

Guia completo: [FCG.Infra](../FCG.Infra/README.md).
