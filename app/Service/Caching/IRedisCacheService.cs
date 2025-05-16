namespace app.Service.Caching
{
    public interface IRedisCacheService
    {
        Task<T>? GetData<T>(string key);
        void SetData<T>(string key, T data);
    }
}
