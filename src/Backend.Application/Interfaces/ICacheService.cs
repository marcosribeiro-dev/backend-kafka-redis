using Backend.Domain.Entities;

namespace Backend.Application.Interfaces;

public interface ICacheService
{
    Task SetAsync(string key, OrderResult value);
    Task<OrderResult?> GetAsync(string key);
}
