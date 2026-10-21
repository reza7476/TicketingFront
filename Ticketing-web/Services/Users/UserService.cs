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
        var request = new
        {
            Name = dto.FirstName,
            LastName = dto.LastName,
            Mobile = dto.Mobile,
            Password = dto.Password
        };

        return _apiClient.PostAsync<object, object>(
            "Users/sign-up-client",
            request);
    }
}
