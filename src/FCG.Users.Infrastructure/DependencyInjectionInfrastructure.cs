using FCG.Users.Application.Messaging;
using FCG.Users.Domain.Usuario.Interfaces;
using FCG.Users.Infrastructure.Data;
using FCG.Users.Infrastructure.Data.Repositories;
using FCG.Users.Infrastructure.Messaging;
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

        services.Configure<UserCreatedPublisherConfig>(
            configuration.GetSection("Publishers:UserCreated"));

        services.AddSingleton<IMessageBus, MessageBus>();

        return services;
    }
}
