using Backend.API.Models;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly ICacheService _cacheService;

    public OrderController(ILogger<OrderController> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Busca um pedido processado no cache do Redis pelo seu ID.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns> O resultado do pedido processado ou um erro 404.</returns>
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(string orderId)
    {
        _logger.LogInformation($"Buscando pedido no cache com OrderId: {orderId}");

        var orderResult = await _cacheService.GetAsync(orderId);

        if (orderResult == null)
        {
            _logger.LogWarning($"Pedido {orderId} não encontrado no cache.");

            var errorResponse = new ErrorResponse
            {
                StatusCode = 404,
                Message = $"Pedido com o ID '{orderId}' não encontrado."
            };
            return NotFound(errorResponse);
        }

        return Ok(orderResult);
    }
}
