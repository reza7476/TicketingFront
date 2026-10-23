using System.ComponentModel.DataAnnotations;

namespace Ticketing_web.EntityModels.Organizations.Models;

public class AddOrganizationModel
{
    public string Name { get; set; } = default!;
    public string OrganizationCode { get; set; } = default!;
    public bool IsDefault { get; set; } 
}
