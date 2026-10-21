namespace Ticketing_web.EntityModels.Authentications.Dto;

public class LoginDto
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; }=default!;
}
