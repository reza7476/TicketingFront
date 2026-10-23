using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Users;
using Ticketing_web.Services.Http;

namespace Ticketing_web.Services.Users;

public class UserService : IUserService
{
    private readonly ApiClient _apiClient;

    public UserService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResultDto<string>> AddAsync(AddUserModel model)
    {
        var request = new
        {
            model.FirstName,
            model.LastName,
            model.Mobile,
            model.OrganizationId,
            model.RoleId,
            model.Password
        };
        return await _apiClient.PostAsync<object, string>(
          "users/add", request
          );
    }

    public async Task<ApiResultDto<GetAllDto<GetAllUsersDto>>>
        GetAllForAdministrator(string? search, UserFilterDto? filter)
    {

        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");

        if (!string.IsNullOrWhiteSpace(filter?.Organization))
            queryParams.Add(
                $"filter.Organization={Uri.EscapeDataString(filter.Organization)}");

        if (!string.IsNullOrWhiteSpace(filter?.Role))
            queryParams.Add(
                $"filter.Role={Uri.EscapeDataString(filter.Role)}");

        var queryString = string.Join("&", queryParams);

        var url = "users/all-for-administrator";

        if (!string.IsNullOrEmpty(queryString))
            url += $"?{queryString}";

        return await _apiClient
            .GetAsync<GetAllDto<GetAllUsersDto>>(url);
    }

    public Task<ApiResultDto<object>> RegisterAgent(RegisterUserDto dto)
    {
        var request = new
        {
            Name = dto.FirstName,
            LastName = dto.LastName,
            Mobile = dto.Mobile,
            Password = dto.Password
        };

        return _apiClient.PostAsync<object, object>(
            "Users/sign-up-agent",
            request);
    }

    public Task<ApiResultDto<object>> RegisterClient(RegisterUserDto dto)
    {
        throw new NotImplementedException();
    }
}
