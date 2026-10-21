using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Tickets;

namespace Ticketing_web.Services.Tickets;

public interface ITicketService : IService
{
    Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>> GetAgentTickets(
        int offset,
        int limit);
    
    Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>> GetClientTickets(
        int offset,
        int limit);
    Task<ApiResultDto<object>> Add(AddTicketDto dto);

    Task<ApiResultDto<GetTicketDetailsDto>> GetDetailsForAgent(string id);
    Task<ApiResultDto<object>> ChangeStatusIntoInProgress(ChangeTicketStatusByAgentDto dto);
    Task <ApiResultDto<object>>ReplyToTicketByAgent(ReplyTicketDto dto);
}
