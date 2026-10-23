using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Roles;
using Ticketing_web.Services.Http;

namespace Ticketing_web.Services.Roles;

public class RoleService : IRoleService
{

    private readonly ApiClient _apiClient;

    public RoleService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResultDto<List<GetAllRolesDto>>> GetAll()
    {
        return await _apiClient
            .GetAsync<List<GetAllRolesDto>>("roles/all");
    }
}
