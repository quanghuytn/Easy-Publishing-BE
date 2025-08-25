using EP.Application.Common.DTOs.Ticket;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<IEnumerable<TicketDto>> GetAllTickets();
    }
}
