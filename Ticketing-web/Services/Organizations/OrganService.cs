using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Organizations;
using Ticketing_web.EntityModels.Organizations.Models;
using Ticketing_web.Services.Http;

namespace Ticketing_web.Services.Organizations;

public class OrganService : IOrganService
{
    private readonly ApiClient _apiClient;

    public OrganService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResultDto<string>> Create(AddOrganizationModel dto)
    {
        var request = new
        {
            dto.IsDefault,
            dto.Name,
            dto.OrganizationCode
        };

        return await _apiClient.PostAsync<object, string>(
            "organizations", request
            );
    }

    public async Task<ApiResultDto<object>> Edit(string id, EditOrganizationModel dto)
    {
        var request = new
        {
            dto.IsDefault,
            dto.Name,
            dto.OrganizationCode
        };
        return await _apiClient
            .PutAsync<object, object>($"organizations/{id}", request);
    }

    public async Task<ApiResultDto<List<GetAllOrganizationDto>>>
        GetAll(string? search = null)
    {
        return await _apiClient.GetAsync<List<GetAllOrganizationDto>>(
            $"organizations/all?search={search}"
            );
    }

    public async Task<ApiResultDto<GetOrganModel?>> GetDetails(string id)
    {
        return await _apiClient.GetAsync<GetOrganModel?>(
            $"organizations/{id}");
    }
}
