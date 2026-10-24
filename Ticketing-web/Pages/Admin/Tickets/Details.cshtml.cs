using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Admin.Tickets;

public class DetailsModel : PageModel
{
    private readonly ITicketService _ticketService;

    public DetailsModel(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    public GetTicketDetailsDto? Ticket { get; private set; }

    [TempData]
    public string? Message { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "شناسه کاربر مقصد الزامی است.")]
    public string AssignedUserId { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "شناسه پاسخ الزامی است.")]
    public string ReplyId { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "متن پاسخ الزامی است.")]
    public string EditedContent { get; set; } = string.Empty;


    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return NotFound();
        await LoadTicketAsync();
        if (Ticket == null)
            return NotFound();
        return Page();
    }

    private async Task LoadTicketAsync()
    {
        Ticket = await _ticketService.GetDetailsForAdminAsync(Id);
    }
}
