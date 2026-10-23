namespace Ticketing_web.EntityModels.Users;

public class AddUserModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? OrganizationId { get; set; }
    public string? RoleId { get; set; }
}
