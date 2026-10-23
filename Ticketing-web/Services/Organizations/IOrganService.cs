using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Organizations;
using Ticketing_web.EntityModels.Organizations.Models;

namespace Ticketing_web.Services.Organizations;

public interface IOrganService : IService
{
    Task<ApiResultDto<List<GetAllOrganizationDto>>>
        GetAll(string? search=null);

    Task<ApiResultDto<string>> Create(AddOrganizationModel dto);
    Task<ApiResultDto<GetOrganModel?>> GetDetails(string id);
    Task<ApiResultDto<object>> Edit(string id, EditOrganizationModel dto);
}
