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



## CRIAÇÃO DO BANCO
docker run -d --name fcg-postgres -p 5435:5432 -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=postgres -v fcg-postgres-data:/var/lib/postgresql/data postgres:16


## CRIAÇÃO DE RABBITMQ E CONFIG´S
docker run -d --name fcg-rabbitmq -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=guest -v fcg-rabbitmq-data:/var/lib/rabbitmq rabbitmq:3.13-management

$rmq = "fcg-rabbitmq"
docker exec $rmq rabbitmqadmin declare exchange name=fcg.user.created-exchange type=direct durable=true
docker exec $rmq rabbitmqadmin declare exchange name=fcg.order.placed-exchange type=direct durable=true
docker exec $rmq rabbitmqadmin declare exchange name=fcg.payment.processed-exchange type=direct durable=true
docker exec $rmq rabbitmqadmin declare queue name=notifications.user-created-queue durable=true
docker exec $rmq rabbitmqadmin declare queue name=payments.order-placed-queue durable=true
docker exec $rmq rabbitmqadmin declare queue name=catalog.payment-processed-queue durable=true
docker exec $rmq rabbitmqadmin declare queue name=notifications.payment-processed-queue durable=true
docker exec $rmq rabbitmqadmin declare binding source=fcg.user.created-exchange destination=notifications.user-created-queue routing_key=notifications.user-created
docker exec $rmq rabbitmqadmin declare binding source=fcg.order.placed-exchange destination=payments.order-placed-queue routing_key=payments.order-placed
docker exec $rmq rabbitmqadmin declare binding source=fcg.payment.processed-exchange destination=catalog.payment-processed-queue routing_key=catalog.payment-processed
docker exec $rmq rabbitmqadmin declare binding source=fcg.payment.processed-exchange destination=notifications.payment-processed-queue routing_key=notifications.payment-processed