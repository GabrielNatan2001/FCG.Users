using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Users.Infrastructure.Messaging;

public static class RabbitMqTopologyExtensions
{
    public static IServiceCollection AddRabbitMqTopologyInitializer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MessageTopologyOptions>(options =>
        {
            ConfigureFromPublishers(configuration, options);
            ConfigureFromWorkers(configuration, options);
        });

        services.AddHostedService<RabbitMqTopologyInitializer>();
        return services;
    }

    private static void ConfigureFromPublishers(IConfiguration configuration, MessageTopologyOptions options)
    {
        foreach (var publisher in configuration.GetSection("Publishers").GetChildren())
        {
            var exchange = publisher["Exchange"];
            if (string.IsNullOrEmpty(exchange))
                continue;

            var routingKey = publisher["RoutingKey"];
            if (!string.IsNullOrEmpty(routingKey))
                AddEntry(options, exchange, routingKey);

            foreach (var key in new[] { "CatalogRoutingKey", "NotificationsRoutingKey" })
            {
                var additionalRoutingKey = publisher[key];
                if (!string.IsNullOrEmpty(additionalRoutingKey))
                    AddEntry(options, exchange, additionalRoutingKey);
            }
        }
    }

    private static void ConfigureFromWorkers(IConfiguration configuration, MessageTopologyOptions options)
    {
        foreach (var worker in configuration.GetSection("Workers").GetChildren())
        {
            var exchange = worker["Exchange"];
            var routingKey = worker["RoutingKey"];

            if (!string.IsNullOrEmpty(exchange) && !string.IsNullOrEmpty(routingKey))
                AddEntry(options, exchange, routingKey);
        }
    }

    private static void AddEntry(MessageTopologyOptions options, string exchange, string routingKey)
    {
        if (options.Entries.Any(e => e.Exchange == exchange && e.RoutingKey == routingKey))
            return;

        options.Entries.Add(new MessageTopologyEntry
        {
            Exchange = exchange,
            RoutingKey = routingKey
        });
    }
}
