namespace Ticketing_web.EntityModels.Users;

public class RegisterUserDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Mobile { get; set; } = default!;
    public string Password { get; set; } = default!;
}
