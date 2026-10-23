using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Roles;

namespace Ticketing_web.Services.Roles;

public interface IRoleService : IService
{
    Task<ApiResultDto<List<GetAllRolesDto>>> GetAll();
}
