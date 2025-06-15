namespace app.DTOs.Auth
{
    public class AccountDto
    {
        public int UserId { get; set; }
        public string Role { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? UserFullname { get; set; }
        public string Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Status { get; set; }
        public string? UserImage { get; set; }
        public string? DescriptionMarkdown { get; set; }
        public string? DescriptionHTML { get; set; }
        public decimal TLT { get; set; }
    }
}
