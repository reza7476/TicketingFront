using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Users;
using Ticketing_web.Pages.Users;

namespace Ticketing_web.Services.Users;

public interface IUserService : IService
{
    Task<ApiResultDto<string>> AddAsync(AddUserModel request);
    Task<ApiResultDto<List<AgentOptionDto>>> GetAllAgents();
    Task<ApiResultDto<GetAllDto<GetAllUsersDto>>>
        GetAllForAdministrator(string? search=null,
        UserFilterDto? filter = null);
    Task<ApiResultDto<GetUserDetailsModel?>> GetDetails(string id);
    Task<ApiResultDto<GetUserProfileDto?>> GetProfile();
    Task<ApiResultDto<object>> RegisterAgent(RegisterUserDto dto);
    Task<ApiResultDto<object>> RegisterClient(RegisterUserDto dto);
    Task<ApiResultDto<object>> UpdateUser(string id, UpdateUserRequest request);
}
