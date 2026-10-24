namespace Ticketing_web.EntityModels.Tickets;

public class AgentAllTicketDto
{
    public string Id { get; set; } = default!;

    public int Number { get; set; }

    public string Title { get; set; } = default!;

    public string AskedBy { get; set; } = default!;

    public Priority Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime AssignedAt { get; set; }

    public TicketStatus Status { get; set; }

}
