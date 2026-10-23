using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Agent.Tickets
{
    public class IndexModel : PageModel
    {
        private readonly ITicketService _ticketService;
        public GetAllDto<GetAllTicketsDto> Tickets { get; set; } = new();

        public IndexModel(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IActionResult> OnGet(
            int pageNumber = 0, 
            int limit = 10)
        {

            var offset = pageNumber * limit;
            var result = await _ticketService.GetAllTicketsForAdmin(offset, limit);


            if (result.IsSuccess && result.Data != null)
            {
                Tickets = result.Data;
                Tickets.CurrentPage = pageNumber;
                Tickets.TotalPages= (int)Math.Ceiling((double)result.Data.TotalElements / limit);
              
            }

            return Page();
        }
    }
}
