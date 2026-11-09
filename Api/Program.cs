using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TokenRevocationService.Application.Interfaces;
using TokenRevocationService.Application.Services;
using TokenRevocationService.Infrastructure;
using TokenRevocationService.Infrastructure.Postgres;
using TokenRevocationService.Infrastructure.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

var storeProvider = builder.Configuration.GetValue<string>("RevocationStore") ?? "Postgres";

if (storeProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
{
    var postgresConn = builder.Configuration.GetConnectionString("PostgresConnection");
    builder.Services.AddDbContext<TokenRevocationDbContext>(options =>
    {
        options.UseNpgsql(postgresConn, npgsql =>
        {
            npgsql.EnableRetryOnFailure(maxRetryCount: 3);
        });
    });

    builder.Services.AddScoped<ITokenRevocationStore, PostgresTokenRevocationStore>();
    builder.Services.AddHostedService<TokenCleanupBackgroundService>();
}
else
{
    var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn));
    builder.Services.AddScoped<ITokenRevocationStore, RedisTokenRevocationStore>();
}

builder.Services.AddScoped<ITokenRevocationService, TokenRevocationService.Application.Services.TokenRevocationService>();

var app = builder.Build();

if (storeProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TokenRevocationDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
