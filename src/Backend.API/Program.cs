using StackExchange.Redis;
using Backend.Application.Interfaces;
using Infrastructure.Services;
using Backend.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o ConnectionMultiplexer do Redis como Singleton devido ser a forma recomendada para reutilizar a conexão.
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(connectionString);
});

// Usamos o serviço de cache como 'Scoped' para que uma nova instância seja criada
// para cada requisição HTTP, garantindo o isolamento dos dados entre as requisições
builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddHostedService<KafkaConsumerService>();

// --- Construção e Pipeline da Aplicação ---
var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();