namespace app.DTOs.Auth
{
    public class UserProfileForm
    {
        public string? UserFullname { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? DescriptionMarkdown { get; set; }
        public string? DescriptionHTML { get; set; }
    }
}
