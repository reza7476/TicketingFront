namespace Ticketing_web.EntityModels.Users;

public class GetAllUsersDto
{
    public string Id { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Mobile { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string Organization { get; set; } = default!;
    public string? Role { get; set; }
}
