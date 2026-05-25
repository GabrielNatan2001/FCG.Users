using FCG.Users.Application.Usuario.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Users.Application;

public static class DependencyInjectionApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CriarUsuarioService>();
        services.AddScoped<AutenticarUsuarioService>();
        return services;
    }
}
