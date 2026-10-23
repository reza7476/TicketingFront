namespace Ticketing_web.EntityModels.Organizations;

public class GetAllOrganizationDto
{
    public string Name { get; set; } = default!;
    public bool IsDefault { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string OrganizationCode { get; set; } = default!;
}
