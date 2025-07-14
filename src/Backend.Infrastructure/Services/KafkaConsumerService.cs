using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Services;

// Usamos o BackgroundService devido ao nosso consumidor rodar em segundo plano por muito tempo.
public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = _configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var topic = _configuration["Kafka:TopicName"];

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(topic);
        _logger.LogInformation($"Kafka Consumer está ouvindo o tópico {topic}");

        // Usamos esse loop para continuar consumindo mensagens de forma contínua.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var order = JsonConvert.DeserializeObject<Backend.Domain.Entities.Order>(consumeResult.Message.Value);

                if (order?.OrderId != null)
                {
                    _logger.LogInformation("Pedido recebido: ", order.OrderId);

                    // Aplica a regra de negócio do desafio: imposto fixo de 10% sobre o valor do pedido.
                    var tax = order.Amount * 0.10m;
                    var total = order.Amount + tax;

                    var orderResult = new OrderResult
                    {
                        OrderId = order.OrderId,
                        CustomerName = order.CustomerName,
                        Amount = order.Amount,
                        Tax = tax,
                        Total = total
                    };

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                        await cacheService.SetAsync(orderResult.OrderId, orderResult);
                        _logger.LogInformation("Pedido processado e salvo no cache:", orderResult.OrderId);
                    }
                }
            }
            // Capturamos exceptions para garantir que o consumidor não pare de rodar case haja erro no processamento.
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Erro ao deserializar a mensagem do Kafka. A mensagem pode não ser um JSON válido.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consumir ou processar mensagem do Kafka.");
            }
        }
        consumer.Close();
    }
}
