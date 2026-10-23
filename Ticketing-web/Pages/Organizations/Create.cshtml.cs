using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Organizations;
using Ticketing_web.EntityModels.Organizations.Models;
using Ticketing_web.Services.Organizations;

namespace Ticketing_web.Pages.Organizations;

public class CreateModel : PageModel
{


    private readonly IOrganService _organService;

    [BindProperty]
    public CreateOrganizationInputDto Input { get; set; } = default!;
    public string? Message { get; set; }
    public CreateModel(IOrganService organService)
    {
        _organService = organService;
    }

    public void OnGet()
    {
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var result = await _organService.Create(new AddOrganizationModel()
        {
            IsDefault = Input.IsDefault,
            Name = Input.Name,
            OrganizationCode = Input.OrganizationCode
        });

        if (result.IsSuccess)
        {
            return RedirectToPage("./Index");
        }
        Message = result.Error ?? "ثبت ارگان با خطا مواجه شد.";

        return Page();
    }
}
