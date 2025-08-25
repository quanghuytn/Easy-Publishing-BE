using EP.Application.Common;
using EP.Application.Common.DTOs.Ticket;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Tickets
{
    public record GetAllTicketQuery : IRequest<ApiResponse<IEnumerable<TicketDto>>>;
    public class GetAllTicketQueryHandler : IRequestHandler<GetAllTicketQuery, ApiResponse<IEnumerable<TicketDto>>>
    {
        private readonly ITicketRepository _ticketRepository;
        public GetAllTicketQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        public async Task<ApiResponse<IEnumerable<TicketDto>>> Handle(GetAllTicketQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketRepository.GetAllTickets();

            return ApiResponse<IEnumerable<TicketDto>>.Success("Tất cả ticket", tickets);
        }
    }
}
