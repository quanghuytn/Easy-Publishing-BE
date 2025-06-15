namespace app.DTOs.Auth
{
    public class ResetPasswordForm
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
