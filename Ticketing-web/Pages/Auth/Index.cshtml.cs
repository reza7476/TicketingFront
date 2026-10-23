using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Authentications.Dto;
using Ticketing_web.Services.Auth;

namespace Ticketing_web.Pages.Auth;

public class IndexModel : PageModel
{
    private readonly IAuthService _authService;


    public IndexModel(
        IAuthService authService)
    {
        _authService = authService;
    }


    [BindProperty]
    public string UserType { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostLogin(
        string mobile,
        string password
        )
    {
        if (string.IsNullOrWhiteSpace(mobile) ||
            string.IsNullOrWhiteSpace(password)
            )
        {
            return new JsonResult(new
            {
                success = false,
                message = "تکمیل تمام فیلدها الزامی است."
            });

        }
        var dto = new LoginDto()
        {
            UserName = mobile,
            Password = password
        };

        var result = await _authService.Login(dto);
        if (result.IsSuccess && result.Data != null)
        {
            HttpContext.Session.Remove("JwtToken");
            HttpContext.Session.SetString("JwtToken", result.Data.Token);
            return new JsonResult(new
            {
                success = true,

            });
        }
        else
        {
            return new JsonResult(new
            {
                success = result.IsSuccess,
                message = "نوع حساب کاربری معتبر نیست"
            });
        }
    }
}
