# FCG.Users

Microsserviço responsável pelo **cadastro de usuários** e **autenticação JWT**. Após criar um usuário, publica o evento `UserCreatedEvent` no RabbitMQ para que o serviço de notificações envie o e-mail de boas-vindas.

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

Em Docker Compose e Kubernetes, use `__` (dois underscores) no lugar de `:` da hierarquia do `appsettings`.

| Variável (Docker/K8s) | appsettings | Obrigatória | Descrição | Exemplo |
|---|---|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | — | Sim | Ambiente de execução | `Production` |
| `ConnectionStrings__DefaultConnection` | `ConnectionStrings:DefaultConnection` | Sim | String de conexão PostgreSQL (`fcg_users`) | `Host=postgres;Port=5432;Database=fcg_users;Username=postgres;Password=postgres` |
| `MessageBusConfigs__Host` | `MessageBusConfigs:Host` | Sim | URI do RabbitMQ | `amqp://admin:admin@rabbitmq:5672/` |
| `MessageBusConfigs__RetryCount` | `MessageBusConfigs:RetryCount` | Não | Tentativas de reconexão ao broker | `5` |
| `Publishers__UserCreated__Exchange` | `Publishers:UserCreated:Exchange` | Sim | Exchange do evento de usuário criado | `fcg.user.created` |
| `Publishers__UserCreated__RoutingKey` | `Publishers:UserCreated:RoutingKey` | Sim | Routing key do evento | `notifications.user-created` |
| `Jwt__Key` | `Jwt:Key` | Sim | Chave simétrica do token (mesma do Catalog) | *(secret)* |
| `Jwt__Issuer` | `Jwt:Issuer` | Sim | Emissor do token | `FCG.Users.API` |
| `Jwt__Audience` | `Jwt:Audience` | Sim | Audiência do token | `FCG.Client` |

## Executar localmente

Pré-requisitos: PostgreSQL, RabbitMQ e demais serviços da plataforma. Para subir tudo com Docker, use o repositório [FCG.Infra](../FCG.Infra).

```bash
dotnet ef database update --project src/FCG.Users.Infrastructure --startup-project src/FCG.Users.API
dotnet run --project src/FCG.Users.API
```

- Swagger: http://localhost:5001/swagger
- Usuário admin (seed): `admin@admin.com` / `Teste@123`

## Deploy

Manifests Kubernetes em `k8s/`. Ordem e instruções completas no [README do FCG.Infra](../FCG.Infra/README.md).
