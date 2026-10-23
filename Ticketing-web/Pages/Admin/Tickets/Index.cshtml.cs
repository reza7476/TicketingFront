using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Admin.Tickets;

public class IndexModel : PageModel
{

    private readonly ITicketService _ticketService;

    public IndexModel(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    public GetAllDto<GetAllTicketsDto> Tickets { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int Limit { get; set; } = 10;

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 0;

    [BindProperty(SupportsGet = true)]
    public TicketStatus? Status { get; set; }
     [BindProperty(SupportsGet = true)]
    public Priority? Priority { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? AskedBy { get; set; }



    public async Task<IActionResult> OnGet()
    {

        var offset = PageNumber * Limit;

        var filter = new TicketFilterDto()
        {
            Status = Status,
            Priority = Priority
        };
      
        var result = await _ticketService.GetAllTicketsForAdmin(
            offset,
            Limit,
            filter,
            AskedBy);

        if (result.IsSuccess && result.Data != null)
        {
            Tickets = result.Data;
            Tickets.TotalPages = (int)Math
                .Ceiling((double)result.Data.TotalElements / Limit);
            Tickets.CurrentPage = PageNumber;
        }

        return Page();
    }
}
