using app.DTOs.Story;
using app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
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

        public IDatabase RedisDb => _redisDb;

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

        public async Task<Object> JsonGetAsync(string key)
        {
            try
            {
                var data = await _redisDb.JSON().GetAsync(key);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getData: {ex.Message}");
                return default;
            }
        }


        public async void StringSetAsync<T>(string key, T data, TimeSpan? expiration = null)
        {
            await _redisDb?.StringSetAsync(key, JsonSerializer.Serialize(data), expiration);
        }

        public async Task<List<StoryDTO>> SearchStoriesAsync(string query, int? authorId, int? fromPrice, int? toPrice, int? status,List<int> cates)
        {
            if (!String.IsNullOrEmpty(query))
            {
                query = query.Replace("%", "\\%")
                             .Replace("@", "\\@")
                             .Replace("(", "\\(")
                             .Replace(")", "\\)")
                             .Replace("-", "\\-")
                             .Replace("+", "\\+");
               
                query = string.Join(" ", query
               .Split(' ', StringSplitOptions.RemoveEmptyEntries)
               .Select(word => $"%{word}%"));
            }
            else
            {
                query = "";
            }

            if (cates.Count > 0)
            {
                foreach (var cate in cates)
                {
                    query += $" @CategoryId:{{{cate}}}";
                }
            }

            var searchQuery = new Query(query);
            if (status.HasValue)
            {
                searchQuery.AddFilter(new Query.NumericFilter("Status", status.Value, status.Value));
            }
            if (authorId.HasValue)
            {
                searchQuery.AddFilter(new Query.NumericFilter("StoryAuthor.UserId", authorId.Value, authorId.Value));
            }
            searchQuery.AddFilter(new Query.NumericFilter("StoryPrice", fromPrice.Value, toPrice.Value));

            var stories = new List<StoryDTO>();
            try
            {
                var result = await _redisDb.FT().SearchAsync("story-idx", searchQuery);

                foreach (var doc in result.Documents)
                {
                    var options = new JsonSerializerOptions { IgnoreReadOnlyProperties = true };
                    var stories12 = JsonSerializer.Deserialize<StoryDTO>(doc["json"], options);
                    stories.Add(stories12);
                }
            }
            catch (RedisServerException ex) when (ex.Message.Contains("Syntax error"))
            {
                return new List<StoryDTO>();
            }
            return stories;
        }

        public async Task AddStoryAsync(int storyId,Object story)
        {
            await _redisDb.JSON().SetAsync($"story:{storyId}", "$", story);
        }
    }
}
