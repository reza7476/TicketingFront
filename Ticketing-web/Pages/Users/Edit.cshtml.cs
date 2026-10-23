using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ticketing_web.EntityModels.Users;
using Ticketing_web.Services.Organizations;
using Ticketing_web.Services.Roles;
using Ticketing_web.Services.Users;

namespace Ticketing_web.Pages.Users;

public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IOrganService _organService;

    public EditModel(
        IUserService userService,
        IRoleService roleService,
        IOrganService organService)
    {
        _userService = userService;
        _roleService = roleService;
        _organService = organService;
    }
    [BindProperty]
    public string Id { get; set; } = string.Empty;

    [BindProperty]
    public EditUserInput? Input { get; set; }

    public List<SelectListItem> Organizations { get; set; } = new();

    public List<SelectListItem> Roles { get; set; } = new();

    public string? Message { get; set; }

    public bool IsSuccess { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Id = id;

        // این متد باید جزئیات یک کاربر را از بک‌اند دریافت کند.
        var result = await _userService.GetDetails(id);

        if (!result.IsSuccess || result.Data == null)
        {
            Message = result.Error ?? "اطلاعات کاربر دریافت نشد.";
            return Page();
        }

        Input = new EditUserInput
        {
            FirstName = result.Data.FirstName,
            LastName = result.Data.LastName,
            Mobile = result.Data.Mobile,
            OrganizationId = result.Data.Organization,
            RoleId = result.Data.Role
        };

        await LoadDropdownsAsync();

        return Page();
    }


    private async Task LoadDropdownsAsync()
    {
        var organizationsResult = await _organService.GetAll();

        if (organizationsResult.IsSuccess &&
            organizationsResult.Data != null)
        {
            Organizations = organizationsResult.Data
                .Select(o => new SelectListItem
                {
                    Text = o.Name,
                    Value = o.Id.ToString(),
                    Selected = o.Id.ToString() == Input?.OrganizationId
                })
                .ToList();
        }

        var rolesResult = await _roleService.GetAll();

        if (rolesResult.IsSuccess && rolesResult.Data != null)
        {
            Roles = rolesResult.Data
                .Where(r => !string.Equals(
                    r.Title,
                    "Administrator",
                    StringComparison.OrdinalIgnoreCase))
                .Select(r => new SelectListItem
                {
                    Text = r.Title,
                    Value = r.Id.ToString(),
                    Selected = r.Id.ToString() == Input?.RoleId
                })
                .ToList();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input == null)
        {
            Message = "اطلاعات فرم نامعتبر است.";
            return Page();
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        // نقش انتخاب‌شده را از فهرست نقش‌های مجاز بررسی می‌کنیم.
        var rolesResult = await _roleService.GetAll();

        var selectedRole = rolesResult.Data?
            .FirstOrDefault(r => r.Id.ToString() == Input.RoleId);

        if (selectedRole == null ||
            string.Equals(
                selectedRole.Title,
                "Administrator",
                StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                "Input.RoleId",
                "این نقش قابل انتخاب نیست.");

            await LoadDropdownsAsync();
            return Page();
        }

        // این متد و DTO را با قرارداد واقعی سرویس خودت هماهنگ کن.
        var request = new UpdateUserRequest
        {
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Mobile = Input.Mobile,
            OrganizationId = Input.OrganizationId,
            RoleId = Input.RoleId
        };

        var updateResult = await _userService.UpdateUser(Id, request);

        if (!updateResult.IsSuccess)
        {
            Message = updateResult.Error ?? "ویرایش کاربر ناموفق بود.";
            await LoadDropdownsAsync();
            return Page();
        }

        TempData["SuccessMessage"] = "اطلاعات کاربر با موفقیت ویرایش شد.";

        return RedirectToPage("./Index");
    }


}

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? OrganizationId { get; set; }
    public string? RoleId { get; set; }
}
