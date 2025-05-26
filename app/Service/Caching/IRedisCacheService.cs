namespace app.Service.Caching
{
    public interface IRedisCacheService
    {
        Task<T>? StringGetAsync<T>(string key);
        void StringSetAsync<T>(string key, T data, TimeSpan? expiration = null);
    }
}
