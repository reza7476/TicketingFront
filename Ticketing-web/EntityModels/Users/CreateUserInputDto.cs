using System.ComponentModel.DataAnnotations;

public class CreateUserInputDto
{
    [Required(ErrorMessage = "نام را وارد کنید.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام خانوادگی را وارد کنید.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره موبایل را وارد کنید.")]
    [RegularExpression(
        @"^09\d{9}$",
        ErrorMessage = "شماره موبایل باید ۱۱ رقم و با 09 شروع شود.")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب سازمان الزامی است.")]
    public string? OrganizationId { get; set; }

    [Required(ErrorMessage = "انتخاب نقش الزامی است.")]
    public string? RoleId { get; set; }

    [Required(ErrorMessage = "رمز عبور را وارد کنید.")]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "رمز عبور باید حداقل ۸ کاراکتر باشد.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
