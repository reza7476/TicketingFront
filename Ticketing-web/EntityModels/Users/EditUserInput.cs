using System.ComponentModel.DataAnnotations;

namespace Ticketing_web.EntityModels.Users;

public class EditUserInput
{
    [Required(ErrorMessage = "نام الزامی است.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام خانوادگی الزامی است.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره موبایل الزامی است.")]
    [RegularExpression(
        @"^09\d{9}$",
        ErrorMessage = "شماره موبایل معتبر نیست.")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب سازمان الزامی است.")]
    public string OrganizationId { get; set; } = default!;

    [Required(ErrorMessage = "انتخاب نقش الزامی است.")]
    public string RoleId { get; set; } = default!;
}
