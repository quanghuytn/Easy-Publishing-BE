namespace app.DTOs.Auth
{
    public class LoginForm
    {
        public string EmailOrUsername { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
