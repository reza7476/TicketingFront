namespace Ticketing_web.EntityModels.Tickets;

public class GetAgentTicketDetailsDto
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string AskedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public TicketStatus Status { get; set; }
    public Priority Priority { get; set; }
    public List<TicketRepliesDto> Replies { get; set; } = new();
}
public class TicketRepliesDto
{
    public string Id { get; set; } = default!;
    public string Mobile { get; set; } = default!;    //کسی که پاسخ داده 
    public string Content { get; set; } = default!;
    public string IssuedBy { get; set; } = default!;    //نقش پاسخ دهنده 
    public DateTime CreatedAt { get; set; }
    public bool ApprovedByAdmin { get; set; }
}
