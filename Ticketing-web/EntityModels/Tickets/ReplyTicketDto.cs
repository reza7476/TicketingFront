using System.ComponentModel.DataAnnotations;

namespace Ticketing_web.EntityModels.Tickets;

public class ReplyTicketDto
{
    [Required]
    public string TicketId { get; set; } = default!;

    [Required]
    public string Content { get; set; } = default!;
}

