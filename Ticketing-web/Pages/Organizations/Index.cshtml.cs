using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Organizations;
using Ticketing_web.Services.Organizations;

namespace Ticketing_web.Pages.Organizations
{
    public class IndexModel : PageModel
    {

        private readonly IOrganService _organService;

        public IndexModel(IOrganService organService)
        {
            _organService = organService;
        }

        public List<GetAllOrganizationDto> Organizations { get; set; } = new();
        public string? Search { get; set; }


        public async Task<IActionResult> OnGet(string? search=null )
        {
            
            Search=search ?? string.Empty;
            var result=await _organService.GetAll(Search);
            if(result.IsSuccess && result.Data != null)
            {
                Organizations= result.Data;
            }
            return Page();  

        }
    }
}
