namespace Ticketing_web.EntityModels.Tickets;

public class ChangeTicketStatusByAgentDto
{
    public string Id { get; set; } = default!;
    public TicketStatus Status { get; set; }
}
