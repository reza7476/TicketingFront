using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Organizations;
using Ticketing_web.EntityModels.Roles;
using Ticketing_web.EntityModels.Users;
using Ticketing_web.Services.Organizations;
using Ticketing_web.Services.Roles;
using Ticketing_web.Services.Users;

namespace Ticketing_web.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IOrganService _organService;


    public GetAllDto<GetAllUsersDto> Users { get; set; } = new();
    public List<GetAllOrganizationDto> Organizations { get; set; } = new();
    public List<GetAllRolesDto> Roles { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Organization { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Role { get; set; }

    public IndexModel(
        IUserService userService,
        IRoleService roleService,
        IOrganService organService)
    {
        _userService = userService;
        _roleService = roleService;
        _organService = organService;
    }

    public async Task<IActionResult> OnGet(
        int pageNumber = 0,
        int limit = 10
        )
    {
        var roleResult = await _roleService.GetAll();
        if (roleResult.IsSuccess && roleResult.Data != null)
        {
            Roles = roleResult.Data.ToList();
        }
        var organResult = await _organService.GetAll();
        if (organResult.IsSuccess && organResult.Data != null)
        {
            Organizations = organResult.Data;
        }

        var offset = pageNumber * limit;

        var filter = new UserFilterDto()
        {
            Organization = Organization,
            Role = Role,
        };

        var result = await _userService.GetAllForAdministrator(Search, filter);


        if (result.IsSuccess && result.Data != null)
        {
            Users = result.Data;
            Users.TotalPages = (int)Math
                .Ceiling((double)result.Data.TotalElements / limit);
        }

        return Page();
    }
}
