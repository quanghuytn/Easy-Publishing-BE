using EP.Application.Common.DTOs.Ticket;
using EP.Application.Common.DTOs.User;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(Context context) : base(context)
        {
        }

        public async Task<IEnumerable<TicketDto>> GetAllTickets()
        {
            return await _dbSet
                .Where(t => t.UserId > 0)
                .Include(t => t.User)
                .OrderByDescending(t => t.TicketDate)
                .Select(t => new TicketDto
                {
                    TicketId = t.TicketId,
                    Status = t.Status,
                    Seen = t.Seen,
                    TicketDate = t.TicketDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    User = new UserDto2
                    {
                        Role = t.User.Role.RoleName,
                        Email = t.User.Email,
                        Username = t.User.Username,
                        Gender = t.User.Gender == true ? "Male" : "Female",
                        Address = t.User.Address,
                        Phone = t.User.Phone,
                        Status = t.User.Status == true ? "Active" : "Inactive",
                        UserImage = t.User.UserImage,
                    }
                })
                .ToListAsync();
        }
    }
}
