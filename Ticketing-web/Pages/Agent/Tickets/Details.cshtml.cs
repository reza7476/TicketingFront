using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Agent.Tickets;

public class DetailsModel : PageModel
{
    private readonly ITicketService _ticketService;

    public DetailsModel(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    [BindProperty]
    public string ReplyContent { get; set; } = string.Empty;
    public GetTicketDetailsDto? Ticket { get; set; }

    public bool CanReply { get; set; }


    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGet(string id)
    {

        var result = await _ticketService.GetDetailsForAgent(id);

        if (!await LoadTicketAsync(id))
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostStart(string id)
    {
       
        var dto = new ChangeTicketStatusByAgentDto
        {
            Id = id,
            Status = TicketStatus.InProgress
        };

        var result = await _ticketService.ChangeStatusIntoInProgress(dto);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Error ?? "تغییر وضعیت ناموفق بود.";
        }

        if (!await LoadTicketAsync(id))
            return NotFound();
        return Page();
    }
    private async Task<bool> LoadTicketAsync(string id)
    {
        var result = await _ticketService.GetDetailsForAgent(id);

        if (!result.IsSuccess || result.Data == null)
            return false;

        Ticket = result.Data;
        Ticket.Id = id;

        CanReply = Ticket.Status == TicketStatus.InProgress;

        return true;
    }

    public async Task<IActionResult> OnPostReply(string id)
    {
        if (string.IsNullOrWhiteSpace(ReplyContent))
        {
            ErrorMessage = "متن پاسخ را وارد کنید.";

            if (!await LoadTicketAsync(id))
                return NotFound();

            return Page();
        }

        var request = new ReplyTicketDto()
        {
            Content = ReplyContent,
            TicketId = id
        };

        var result = await _ticketService.ReplyToTicketByAgent(
           request);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Error ?? "ثبت پاسخ ناموفق بود.";

            if (!await LoadTicketAsync(id))
                return NotFound();

            return Page();
        }

        return RedirectToPage(new { id });
    }

}
