using FCG.Users.Domain.Usuario.Interfaces;
using FCG.Users.Infrastructure.Data;
using FCG.Users.Infrastructure.Data.Repositories;
using FCG.Users.Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Users.Infrastructure;

public static class DependencyInjectionInfrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((_, cfg) =>
            {
                RabbitMqBusConfiguration.ConfigureHost(cfg, configuration);
                RabbitMqBusConfiguration.ConfigurePublishOnly(cfg, configuration);
            });
        });

        return services;
    }
}
