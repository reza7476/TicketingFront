using System.ComponentModel.DataAnnotations;

namespace Ticketing_web.EntityModels.Organizations;

public class CreateOrganizationInputDto
{
    [Required(ErrorMessage = "نام ارگان الزامی است.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "کد ارگان الزامی است.")]
    public string OrganizationCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }=false;
}
