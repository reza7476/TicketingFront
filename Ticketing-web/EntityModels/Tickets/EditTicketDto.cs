namespace Ticketing_web.EntityModels.Tickets;

public class EditTicketDto
{
    public string TicketId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public Priority Priority { get; set; }
}
