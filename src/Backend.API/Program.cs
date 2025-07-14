using StackExchange.Redis;
using Backend.Application.Interfaces;
using Infrastructure.Services;
using Backend.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o ConnectionMultiplexer do Redis como Singleton devido ser a forma recomendada para reutilizar a conex�o.
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(connectionString);
});

// Usamos o servi�o de cache como 'Scoped' para que uma nova inst�ncia seja criada
// para cada requisi��o HTTP, garantindo o isolamento dos dados entre as requisi��es
builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddHostedService<KafkaConsumerService>();

// --- Constru��o e Pipeline da Aplica��o ---
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();