using System.ComponentModel.DataAnnotations;

namespace Ticketing_web.EntityModels.Tickets;

public class AddTicketDto
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public Priority Priority { get; set; } = default!;
}
