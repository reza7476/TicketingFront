using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Authentications.Dto;
using Ticketing_web.EntityModels.Users;
using Ticketing_web.Services.Auth;
using Ticketing_web.Services.Users;

namespace Ticketing_web.Pages.Auth;

public class IndexModel : PageModel
{
    private readonly IAuthService _authService;

    private readonly IUserService _userService;

    public IndexModel(
        IAuthService authService, 
        IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }


    [BindProperty]
    public string UserType { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostRegister(
       string FirstName,
       string LastName,
       string Mobile,
       string Password,
       string ConfirmPassword,
       string UserType)
    {
        if (string.IsNullOrWhiteSpace(FirstName) ||
            string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Mobile) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            return new JsonResult(new
            {
                success = false,
                message = "تکمیل تمام فیلدها الزامی است."
            });
        }

        if (Password != ConfirmPassword)
        {
            return new JsonResult(new
            {
                success = false,
                message = "رمز عبور و تکرار آن مطابقت ندارند."
            });
        }

        var dto = new RegisterUserDto
        {
            FirstName = FirstName,
            LastName = LastName,
            Mobile = Mobile,
            Password = Password
        };

        if (UserType == "client")
        {
            var result = await _userService.RegisterClient(dto);

            return new JsonResult(new
            {
                success = result.IsSuccess,
                message = result.Error ?? " "
            });
        }

        if (UserType == "agent")
        {
            var result = await _userService.RegisterAgent(dto);

            return new JsonResult(new
            {
                success = result.IsSuccess,
                message = result.Error ?? " "
            });
        }

        return new JsonResult(new
        {
            success = false,
            message = "نوع حساب کاربری معتبر نیست."
        });
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
