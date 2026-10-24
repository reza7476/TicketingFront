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
        return await _apiClient.PostAsync<object, object>(
            "tickets/add",
            request);
    }

    public async Task<ApiResultDto<object>> AssignTicketAsync(string id, string assignedUserId)
    {
        var request = new
        {
            TicketId = id,
            AgentId = assignedUserId,
        };

        return await _apiClient.PostAsync<object>("tickets/assign-to-agent", request);
    }

    public async Task<ApiResultDto<GetAllDto<AgentAllTicketDto>>>
        GetAllAgentTickets(TicketFilterDto filter)
    {
        var queryParams = new List<string>();

       if(filter != null)
        {
            if (filter.Status != null)
            {
                queryParams.Add($"Status={(int)filter.Status}");
            }

            if (filter.Priority != null)
            {
                queryParams.Add($"Priority={(int)filter.Priority}");
            }
        }
        var queryString = string.Join("&", queryParams);
        return await _apiClient.GetAsync<GetAllDto<AgentAllTicketDto>>($"tickets/all-for-agent{queryString}");
    }

    public async Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>>
    GetAllTicketsForAdmin(
        int offset,
        int limit,
        TicketFilterDto? filter = null,
        string? search = null)
    {
        var queryParams = new List<string>
         {
             $"offset={offset}",
             $"limit={limit}"
         };

        if (!string.IsNullOrEmpty(search))
        {
            queryParams.Add($"search=" + search);
        }

        if (filter != null)
        {
            if (filter.Status != null)
            {
                queryParams.Add($"Status={(int)filter.Status}");
            }

            if (filter.Priority != null)
            {
                queryParams.Add($"Priority={(int)filter.Priority}");
            }
        }

        var queryString = string.Join("&", queryParams);

        return await _apiClient.GetAsync<GetAllDto<GetAllTicketsDto>>(
            $"tickets/get-all-for-admin?{queryString}");
    }

    public async Task<ApiResultDto<GetAllDto<GetAllTicketsDto>>>
        GetClientTickets(int offset, int limit)
    {
        return await _apiClient
           .GetAsync<GetAllDto<GetAllTicketsDto>>(
           $"tickets/all-for-client?offset={offset}&limit={limit}");
    }

    public async Task<ApiResultDto<GetTicketDetailsDto?>>
        GetDetailsForAdminAsync(string id)
    {
        return await _apiClient
             .GetAsync<GetTicketDetailsDto?>($"tickets/{id}/for-admin");
    }

    public async Task<ApiResultDto<GetAgentTicketDetailsDto?>> GetDetailsForAgent(string id)
    {
        return await _apiClient.GetAsync<GetAgentTicketDetailsDto?>($"tickets/{id}/details-for-agent");
    }
}
