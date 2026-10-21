namespace Ticketing_web.EntityModels.Tickets;

public class GetAllTicketsDto
{
    
    public string Id { get; set; } = default!;
    public string AskedBy { get; set; } = default!;
    public string Title { get; set; } = default!;
    public TicketStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public long Number { get; set; }
}
public enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3,
}

public enum TicketStatus
{
    Open = 1,
    InProgress = 2,
    Answered = 3,
    Closed = 4
}
