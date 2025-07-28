namespace app.Interface
{
    public interface IHashService
    {
        string Hash(string password);
        bool Verify(string passwordHash, string passwordInput);
    }
}
