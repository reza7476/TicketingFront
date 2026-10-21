using Ticketing_web.EntityModels.Authentications.Dto;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.Services.Http;

namespace Ticketing_web.Services.Auth;

public class AuthenticationService : IAuthService
{
    private readonly ApiClient _apiClient;

    public AuthenticationService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResultDto<GetTokenDto?>> Login(LoginDto dto)
    {
        var request = new LoginDto
        {
            UserName = dto.UserName,
            Password = dto.Password,
        };

        return await _apiClient.PostAsync<LoginDto, GetTokenDto?>(
            "authentication/log-in",
            request);
    }
}
