using FCG.Users.API.Middlewares;
using FCG.Users.API.Observability;
using FCG.Users.API.Services;
using FCG.Users.Application;
using FCG.Users.Application.Abstractions.Security;
using FCG.Users.Domain.Usuario.Entities;
using FCG.Users.Infrastructure;
using FCG.Users.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddFcgOpenTelemetry("fcg-users-api");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecksInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddScoped<ITokenProvider, JwtTokenProvider>();

var app = builder.Build();
await SeedAdminAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseHttpMetrics();
app.MapHealthChecks("/health");
app.MapMetrics();
app.MapControllers();
app.Run();

static async Task SeedAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    const string adminEmail = "admin@admin.com";
    if (await db.Usuarios.AnyAsync(u => u.Email.Value == adminEmail))
        return;

    await db.Usuarios.AddAsync(UsuarioEntity.CriarAdmin("admin", adminEmail, "Teste@123"));
    await db.SaveChangesAsync();
}
