using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Backend.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<OrderResult?> GetAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return null;
        }
        return JsonConvert.DeserializeObject<OrderResult>(value);
    }

    public async Task SetAsync(string key, OrderResult value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        var expiration = TimeSpan.FromMinutes(30);
        await _database.StringSetAsync(key, jsonValue, expiration);
    }
}
