using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Organizations;
using Ticketing_web.EntityModels.Organizations.Models;
using Ticketing_web.Services.Organizations;

namespace Ticketing_web.Pages.Organizations
{
    public class EditModel : PageModel
    {

        private readonly IOrganService _organService;

        [BindProperty]
        public CreateOrganizationInputDto? Input { get; set; }

        [BindProperty]
        public string Id { get; set; } = default!;
        public string? Message { get; set; }

        public EditModel(IOrganService organService)
        {
            _organService = organService;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var result = await _organService.GetDetails(id);
            if (!result.IsSuccess)
            {
                Message = result.Error ?? "خطایی پیش امده.";

                return Page();
            }
            else if (result.IsSuccess && result.Data != null)
            {
                Input = new CreateOrganizationInputDto
                {
                    IsDefault = result.Data.IsDefault,
                    Name = result.Data.Name,
                    OrganizationCode = result.Data.OrganizationCode
                };
                Id = id;
                return Page();
            }
            else
            {
                Message = "ارگان مورد نظر یافت نشد";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();  
            }
            var result = await _organService.Edit(Id, new EditOrganizationModel()
            {
                IsDefault=Input.IsDefault,
                Name = Input.Name,   
                OrganizationCode = Input.OrganizationCode
            });
            if (result.IsSuccess)
            {
                return RedirectToPage("./Index");
            }
            Message = result.Error ?? "ویرایش ارگان ناموفق بود.";

            return Page();
        }
    }
}
