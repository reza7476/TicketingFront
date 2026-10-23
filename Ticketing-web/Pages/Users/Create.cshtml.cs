using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ticketing_web.EntityModels.Users;
using Ticketing_web.Services.Organizations;
using Ticketing_web.Services.Roles;
using Ticketing_web.Services.Users;

namespace Ticketing_web.Pages.Users;

public class CreateModel : PageModel
{

    private readonly IUserService _userService;
    private readonly IOrganService _organizationService;
    private readonly IRoleService _roleService;
    [BindProperty]
    public CreateUserInputDto Input { get; set; } = new();

    public List<SelectListItem> Organizations { get; set; } = new();

    public List<SelectListItem> Roles { get; set; } = new();

    public string? Message { get; set; }

    public bool IsSuccess { get; set; }
    public CreateModel(
        IUserService userService,
        IOrganService organizationService,
        IRoleService roleService)
    {
        _userService = userService;
        _organizationService = organizationService;
        _roleService = roleService;
    }

    public async Task OnGetAsync()
    {
        await LoadDropdownsAsync();
    }


    private async Task LoadDropdownsAsync()
    {
        var organizationsResult =
            await _organizationService.GetAll();

        Organizations = organizationsResult.Data?
            .Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.Id.ToString(),
                Selected = o.Id.ToString() == Input.OrganizationId
            })
            .ToList() ?? new List<SelectListItem>();

        var rolesResult = await _roleService.GetAll();

        Roles = rolesResult.Data?
            .Where(r => !string.Equals(
                r.Title,
                "Administrator",
                StringComparison.OrdinalIgnoreCase))
            .Select(r => new SelectListItem
            {
                Text = r.Title,
                Value = r.Id.ToString(),
                Selected = r.Id.ToString() == Input.RoleId
            })
            .ToList() ?? new List<SelectListItem>();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // اگر اعتبارسنجی فرم ناموفق بود،
        // فهرست‌های کشویی باید دوباره بارگذاری شوند.
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        // از اختصاص نقش Administrator از سمت فرانت جلوگیری می‌کنیم.
        var rolesResult = await _roleService.GetAll();

        var selectedRole = rolesResult.Data?
            .FirstOrDefault(r => r.Id == Input.RoleId);

        if (selectedRole is null ||
            string.Equals(
                selectedRole.Title,
                "Administrator",
                StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                "Input.RoleId",
                "انتخاب این نقش مجاز نیست.");

            await LoadDropdownsAsync();
            return Page();
        }

        var request = new AddUserModel
        {
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Mobile = Input.Mobile,
            Password = Input.Password,
            OrganizationId = Input.OrganizationId,
            RoleId = Input.RoleId
        };

        var result = await _userService.AddAsync(request);

        if (!result.IsSuccess)
        {
            Message = result.Error ?? "ثبت کاربر ناموفق بود.";
            IsSuccess = false;

            await LoadDropdownsAsync();
            return Page();
        }

        TempData["SuccessMessage"] = "کاربر با موفقیت ثبت شد.";

        return RedirectToPage("./Index");
    }

}
