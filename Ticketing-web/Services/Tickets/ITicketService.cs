using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Tickets;

namespace Ticketing_web.Services.Tickets;

public interface ITicketService : IService
{

    
    Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>> GetClientTickets(
        int offset,
        int limit);

    Task<ApiResultDto<object>> Add(AddTicketDto dto);
   
    Task<ApiResultDto<GetTicketDetailsDto?>> 
        GetDetailsForAdminAsync(string id);
  
    
    Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>> 
        GetAllTicketsForAdmin(
        int offset, int limit,
        TicketFilterDto filter,
        string? askedBy);
    Task<ApiResultDto<object>> AssignTicketAsync(string id, string assignedUserId);
}
