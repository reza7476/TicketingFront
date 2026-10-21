using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.EntityModels.Authentications.Dto;
using Ticketing_web.EntityModels.Commons;

namespace Ticketing_web.Services.Auth;

public interface IAuthService : IService
{
    Task<ApiResultDto<GetTokenDto?>> Login(LoginDto dto);
}
