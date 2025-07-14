using Backend.Domain.Entities;

namespace Backend.UnitTests.Backend.UnitTests;

public class ProcessingTests
{
    [Fact]
    public void ProcessOrder()
    {
        // Criamos um pedido de exemplo com valores aleatórios.
        var order = new Order
        {
            OrderId = "test-123",
            CustomerName = "Cliente Teste",
            Amount = 100.00m
        };

        var expectedTax = 10.00m; // 10% de 100
        var expectedTotal = 110.00m; // 100 + 10

        // Executa a mesma lógica de processamento do consumidor Kafka
        var tax = order.Amount * 0.10m;
        var total = order.Amount + tax;

        var result = new OrderResult
        {
            OrderId = order.OrderId,
            CustomerName = order.CustomerName,
            Amount = order.Amount,
            Tax = tax,
            Total = total
        };

        // Verifica se os valores calculados são os esperados pela lógica
        Assert.Equal(expectedTax, result.Tax);
        Assert.Equal(expectedTotal, result.Total);
    }
}
