using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Review
{
    public class ReviewerDto
    {
        public int UserId { get; set; }
        public string? UserFullname { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Username { get; set; } = null!;
        public string? Status { get; set; }
        public string? DescriptionMarkdown { get; set; }
        public string? DescriptionHTML { get; set; }
        public string? UserImage { get; set; }
    }
}
