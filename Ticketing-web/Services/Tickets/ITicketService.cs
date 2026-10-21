using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Tickets;

namespace Ticketing_web.Services.Tickets;

public interface ITicketService : IService
{
    Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>> GetAgentTickets(
        int offset,
        int limit);
}
