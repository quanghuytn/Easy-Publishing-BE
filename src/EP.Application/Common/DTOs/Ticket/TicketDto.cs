using EP.Application.Common.DTOs.User;

namespace EP.Application.Common.DTOs.Ticket
{
    public class TicketDto
    {
        public int TicketId { get; set; }
        public bool? Status { get; set; }
        public bool? Seen { get; set; }
        public string TicketDate { get; set; }
        public UserDto2 User { get; set; }
    }
}
