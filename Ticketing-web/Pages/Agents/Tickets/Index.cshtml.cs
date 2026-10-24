using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Ticketing_web.EntityModels.Commons;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Agents.Tickets;

public class IndexModel : PageModel
{

    private readonly ITicketService _ticketService;
    [BindProperty(SupportsGet = true)]
    public bool? AdminApproval { get; set; }

    [BindProperty(SupportsGet = true)]
    public Priority? Priority { get; set; }

    [BindProperty(SupportsGet = true)]
    public TicketStatus? Status { get; set; }

    public GetAllDto<AgentAllTicketDto> Tickets { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public IndexModel(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    public async Task<IActionResult> OnGet()
    {
        var filter = new TicketFilterDto
        {
            Status = Status,
            Priority = Priority
        };

        var result = await _ticketService.GetAllAgentTickets(filter);


        if (result.IsSuccess && result.Data != null)
        {
            Tickets = result.Data;
            
        }

        return Page();
    }


    public string GetPriorityClass(Priority? priority)
    {
        return priority switch
        {
            Ticketing_web.EntityModels.Tickets.Priority.High => "priority-high",
            Ticketing_web.EntityModels.Tickets.Priority.Medium => "priority-medium",
            Ticketing_web.EntityModels.Tickets.Priority.Low => "priority-low",
            _ => "priority-default"
        };
    }
    public string GetStatusClass(TicketStatus status)
    {
        return status switch
        {
            TicketStatus.Open => "status-open",
            TicketStatus.InProgress => "status-progress",
            TicketStatus.Answered => "status-answered",
            TicketStatus.Closed => "status-closed",
            _ => "status-default"
        };
    }
}
