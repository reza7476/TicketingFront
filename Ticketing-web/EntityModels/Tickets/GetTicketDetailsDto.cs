namespace Ticketing_web.EntityModels.Tickets;


public class GetTicketDetailsDto
{
    public string Content { get; set; } = default!;
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Priority Priority { get; set; }
    public string AskedBy { get; set; } = default!;//user mobile
    public string Title { get; set; } = default!;
    public string OwnerOrgan { get; set; } = default!;//user organ
    public long Number { get; set; }
    public List<TicketRepliedDto> Replies { get; set; } = new();
    public List<AssignmentHistoryDto> AssignmentHistory { get; set; } = new();
}

public class AssignmentHistoryDto
{
    public string AssignedUser { get; set; } = default!;//name last name
    public DateTime CreatedAt { get; set; }
}

public class TicketRepliedDto
{
    public string Id { get; set; } = default!;
    public DateTime RepliedAt { get; set; }
    public string RepliedBy { get; set; } = default!;//name-last name
    public string Content { get; set; } = default!;

    public string IssuedBy { get; set; } = default!;//client agent admin

    public bool ApprovedByAdmin { get; set; }

}


