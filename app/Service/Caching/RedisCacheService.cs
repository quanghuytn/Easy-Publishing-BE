using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace app.Service.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer? _connectionMultiplexer;
        private readonly IDatabase _redisDb;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisDb = connectionMultiplexer.GetDatabase();
        }

        public async Task<T>? StringGetAsync<T>(string key)
        {
            try
            {
                var data = await _redisDb.StringGetAsync(key);
              
                return data.HasValue ? JsonSerializer.Deserialize<T>(data) : default;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error getData: {ex.Message}");
                return default;
            }
        }


        public async void StringSetAsync<T>(string key, T data, TimeSpan? expiration = null)
        {
            await _redisDb?.StringSetAsync(key, JsonSerializer.Serialize(data), expiration);
        }
    }
}
