using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Users;
using Ticketing_web.Services.Users;

namespace Ticketing_web.Pages.Users.Profile;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;

    public IndexModel(IUserService userService)
    {
        _userService = userService; 
        
    }

    public GetUserProfileDto Profile { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {

        var result = await _userService.GetProfile();

        if (result != null && result.Data != null)
        {
            Profile = result.Data;
        }
        return Page();
    }
}
