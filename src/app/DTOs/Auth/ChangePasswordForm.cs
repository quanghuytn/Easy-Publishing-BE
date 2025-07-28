namespace app.DTOs.Auth
{
    public class ChangePasswordForm
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
