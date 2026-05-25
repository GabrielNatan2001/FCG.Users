using FCG.Users.Application.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace FCG.Users.Infrastructure.Messaging;

internal static class RabbitMqBusConfiguration
{
    public static void ConfigureHost(IRabbitMqBusFactoryConfigurator cfg, IConfiguration configuration)
    {
        var rabbit = configuration.GetSection("RabbitMq");
        cfg.Host(
            rabbit["Host"] ?? "localhost",
            ushort.Parse(rabbit["Port"] ?? "5672"),
            rabbit["VirtualHost"] ?? "/",
            h =>
            {
                h.Username(rabbit["Username"] ?? "guest");
                h.Password(rabbit["Password"] ?? "guest");
            });

        cfg.DeployPublishTopology = false;
    }

    public static void ConfigurePublishOnly(IRabbitMqBusFactoryConfigurator cfg, IConfiguration _)
    {
        cfg.Publish<UserCreatedEvent>(p => p.ExchangeType = ExchangeType.Fanout);
    }
}
