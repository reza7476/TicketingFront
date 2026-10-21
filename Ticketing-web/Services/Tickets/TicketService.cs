using Ticketing_web.EntityModels.Authentications.Dto;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Http;

namespace Ticketing_web.Services.Tickets;

public class TicketService : ITicketService
{
    private readonly ApiClient _apiClient;

    public TicketService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResultDto<object>> Add(AddTicketDto dto)
    {
        var request = new
        {
            dto.Priority,
            dto.Content,
            dto.Title,
        };
        return await _apiClient.PostAsync<object,object>(
            "tickets/add",
            request);
    }

    public async Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>>
        GetAgentTickets(int offset, int limit)
    {
        return await _apiClient
            .GetAsync<GetAllDto<GetAllTicketsDto>>(
            $"tickets/get-all-for-agent?offset={offset}&limit={limit}");
    }

    public  async Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>>
        GetClientTickets(int offset, int limit)
    {
        return await _apiClient
           .GetAsync<GetAllDto<GetAllTicketsDto>>(
           $"tickets/all-for-client?offset={offset}&limit={limit}");
    }
}
