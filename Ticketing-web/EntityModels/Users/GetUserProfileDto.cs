namespace Ticketing_web.EntityModels.Users;

public class GetUserProfileDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Mobile { get; set; } = default!;
    public string OrganName { get; set; } = default!;
}
