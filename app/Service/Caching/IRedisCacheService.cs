using app.DTOs;

namespace app.Service.Caching
{
    public interface IRedisCacheService
    {
        Task<T>? StringGetAsync<T>(string key);
        Task<Object> JsonGetAsync(string key);
        void StringSetAsync<T>(string key, T data, TimeSpan? expiration = null);
        Task AddStoryAsync(int storyId, Object story);
        Task<List<StoryDTO>> SearchStoriesAsync(string query, int? authorId, int? fromPrice, int? toPrice, int? status, List<int> cates);
    }
}
