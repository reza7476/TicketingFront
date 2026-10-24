using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Replies;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Clients.Tickets;

public class DetailsModel : PageModel
{
    private readonly ITicketService _ticketService;
    private readonly ITicketReplyService _replyService;


    public DetailsModel(
        ITicketService ticketService,
        ITicketReplyService replyService)
    {
        _ticketService = ticketService;
        _replyService = replyService;
    }
    public string Id { get; private set; } = string.Empty;

    public GetClientTicketDetailsDto? Ticket { get; private set; }
    [BindProperty]
    public string ReplyContent { get; set; } = string.Empty;

    [BindProperty]
    public string ReplyId { get; set; } = string.Empty;

    [BindProperty]
    public string EditedContent { get; set; } = string.Empty;

    public string? Message { get; private set; }

    public string? ErrorMessage { get; private set; }
    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        Id = id;
        Message = TempData["Message"] as string;

        await LoadTicketAsync();

        if (Ticket is null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostReplyAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        Id = id;

        if (!IsValidContent(ReplyContent))
        {
            ErrorMessage = "متن پاسخ باید بین ۲ تا ۵۰۰۰ کاراکتر باشد.";
            await LoadTicketAsync();
            return Page();
        }

        await LoadTicketAsync();

        if (Ticket is null)
            return NotFound();

        if (Ticket.Status != TicketStatus.Answered)
        {
            ErrorMessage = "در وضعیت فعلی امکان ثبت پاسخ وجود ندارد.";
            return Page();
        }

        var result = await _replyService.ReplyTicket(
            Id,
            ReplyContent);

        if (result is null || !result.IsSuccess)
        {
            ErrorMessage = result?.Error ?? "ثبت پاسخ ناموفق بود.";
            return Page();
        }

        TempData["Message"] = "پاسخ شما با موفقیت ثبت شد.";

        return RedirectToPage("./Details", new { id = Id });
    }
    public async Task<IActionResult> OnPostEditReplyAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id) ||
            string.IsNullOrWhiteSpace(ReplyId))
        {
            return NotFound();
        }

        Id = id;

        if (!IsValidContent(EditedContent))
        {
            ErrorMessage = "متن پاسخ باید بین ۲ تا ۵۰۰۰ کاراکتر باشد.";
            await LoadTicketAsync();
            return Page();
        }

        await LoadTicketAsync();

        if (Ticket is null)
            return NotFound();

        if (Ticket.Status != TicketStatus.Answered)
        {
            ErrorMessage = "در وضعیت فعلی امکان ویرایش پاسخ وجود ندارد.";
            return Page();
        }

        var reply = Ticket.Replies.FirstOrDefault(x => x.Id == ReplyId);

        if (reply is null)
        {
            ErrorMessage = "پاسخ موردنظر پیدا نشد.";
            return Page();
        }

        

        var result = await _replyService.EditReply(
            
            ReplyId,
            EditedContent.Trim());

        if (result is null || !result.IsSuccess)
        {
            ErrorMessage = result?.Error ?? "ویرایش پاسخ ناموفق بود.";
            return Page();
        }

        TempData["Message"] = "پاسخ با موفقیت ویرایش شد.";

        return RedirectToPage("./Details", new { id = Id });
    }

    private async Task LoadTicketAsync()
    {
        var result = await _ticketService.GetClientTicketDetails(Id);

        if (result is null || !result.IsSuccess || result.Data is null)
        {
            Ticket = null;
            ErrorMessage ??= result?.Error ?? "دریافت اطلاعات تیکت ناموفق بود.";
            return;
        }

        Ticket = result.Data;
    }
    private static bool IsValidContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        var length = content.Trim().Length;

        return length >= 2 && length <= 5000;
    }
}
