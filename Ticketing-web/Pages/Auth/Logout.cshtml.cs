using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ticketing_web.Pages.Auth;

public class LogoutModel : PageModel
{
    public void OnGet()
    {
    }
    public IActionResult OnPost()
    {
        HttpContext.Session.Clear();

        return RedirectToPage("/Auth/Index");
    }
}
