namespace Ticketing_web.EntityModels.Users;

public class GetUserDetailsModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Organization { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string OrganizationId { get; set; } = default!;
    public string RoleId { get; set; } = default!;
}

