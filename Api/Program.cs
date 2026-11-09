using StackExchange.Redis;
using TokenRevocationService.Application.Interfaces;
using TokenRevocationService.Application.Services;
using TokenRevocationService.Infrastructure.Redis;

var builder = WebApplication.CreateBuilder(args);

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis")
        ?? "localhost:6379";

    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddScoped<ITokenRevocationStore, RedisTokenRevocationStore>();
builder.Services.AddScoped<ITokenRevocationService, TokenRevocationService.Application.Services.TokenRevocationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Token Revocation Service v1"));
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();