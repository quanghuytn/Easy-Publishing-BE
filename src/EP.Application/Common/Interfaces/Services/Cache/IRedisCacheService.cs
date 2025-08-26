using System.Runtime.CompilerServices;

namespace EP.Application.Common.Interfaces.Services.Cache
{
    public interface IRedisCacheService
    {
        Task<T>? StringGetAsync<T>(string key);
        Task<object> JsonGetAsync(string key);
        Task StringSetAsync<T>(string key, T data, TimeSpan? expiration = null);
        Task AddStoryAsync(int storyId, object story);
        //Task<List<StoryDto>> SearchStoriesAsync(string query, int? authorId, int? fromPrice, int? toPrice, int? status, List<int> cates);
    }
}
