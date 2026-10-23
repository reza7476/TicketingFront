namespace Ticketing_web.EntityModels.Tickets;

public class TicketFilterDto
{
    public Priority? Priority { get; set; }
    public TicketStatus? Status { get; set; }
}
