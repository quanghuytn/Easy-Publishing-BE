namespace EP.Application.Common.Interfaces.Services.Common
{
    public interface IHashService
    {
        string Hash(string password);
        bool Verify(string passwordHash, string passwordInput);
        string HmacSHA512(string key, string inputData);
        string HmacSHA256(string inputData, string key);
    }
}
