using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Replies;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Agents.Tickets;

public class DetailsModel : PageModel
{

    private readonly ITicketService _ticketService;
    private readonly ITicketReplyService _replyService;
    public GetAgentTicketDetailsDto? Ticket { get; private set; }

    public string Id { get; private set; } = string.Empty;

    public string? Message { get; private set; }

    public string? ErrorMessage { get; private set; }

    public DetailsModel(
        ITicketService ticketService,
        ITicketReplyService replyService)
    {
        _ticketService = ticketService;
        _replyService = replyService;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        Id = id;

        await LoadTicketAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostEditReplyAsync(
        string id,
       string replyId,
       string editedContent)
    {
        if (string.IsNullOrWhiteSpace(replyId))
        {
            return NotFound();
        }
        Id = id;


        if (string.IsNullOrWhiteSpace(editedContent)
            || editedContent.Trim().Length < 2
            || editedContent.Length > 5000)
        {
            ErrorMessage = "متن پاسخ باید بین ۲ تا ۵۰۰۰ کاراکتر باشد.";
            await LoadTicketAsync();
            return Page();
        }

        await LoadTicketAsync();

        if (Ticket == null)
        {
            ErrorMessage = "تیکت پیدا نشد.";
            return Page();
        }

        if (Ticket.Status != TicketStatus.InProgress)
        {
            ErrorMessage = "وضعیت تیکت اجازه ویرایش پاسخ را نمی‌دهد.";
            return Page();
        }

        var reply = Ticket.Replies.FirstOrDefault(x => x.Id == replyId);

        if (reply == null)
        {
            ErrorMessage = "پاسخ موردنظر پیدا نشد.";
            return Page();
        }

        var isAgentReply = string.Equals(
            reply.IssuedBy,
            "agent",
            StringComparison.OrdinalIgnoreCase);

        if (!isAgentReply || reply.ApprovedByAdmin)
        {
            ErrorMessage = "این پاسخ قابل ویرایش نیست.";
            return Page();
        }

        var result = await _replyService.EditReply(
            replyId,
            editedContent);

        if (result == null || !result.IsSuccess)
        {
            ErrorMessage = "ویرایش پاسخ ناموفق بود.";
            await LoadTicketAsync();
            return Page();
        }

        TempData["Message"] = "پاسخ با موفقیت ویرایش شد.";

        return RedirectToPage("./Details", new { Id });
    }

    private async Task LoadTicketAsync()
    {
        Message = TempData["Message"] as string;

        var result = await _ticketService.GetDetailsForAgent(Id);

        if (result == null || !result.IsSuccess || result.Data == null)
        {
            Ticket = null;
            ErrorMessage ??= "دریافت اطلاعات تیکت ناموفق بود.";
            return;
        }

        Ticket = result.Data;
    }




    public async Task<IActionResult> OnPostReplyAsync(
       string id,
       string replyContent)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        Id = id;

        if (string.IsNullOrWhiteSpace(replyContent)
            || replyContent.Trim().Length < 2
            || replyContent.Length > 5000)
        {
            ErrorMessage = "متن پاسخ باید بین ۲ تا ۵۰۰۰ کاراکتر باشد.";
            await LoadTicketAsync();
            return Page();
        }

        await LoadTicketAsync();

        if (Ticket == null)
        {
            ErrorMessage = "تیکت پیدا نشد.";
            return Page();
        }

        if (Ticket.Status != TicketStatus.InProgress)
        {
            ErrorMessage = "وضعیت تیکت اجازه ثبت پاسخ نمی‌دهد.";
            return Page();
        }

        if (Ticket.Replies.Count > 0)
        {
            ErrorMessage = "این تیکت قبلاً پاسخ دارد و نمی‌توان از این فرم پاسخ جدید ثبت کرد.";
            return Page();
        }

        var result = await _replyService.ReplyByAgent(
            id,
            replyContent);

        if (result == null || !result.IsSuccess)
        {
            ErrorMessage = "ثبت پاسخ ناموفق بود.";
            await LoadTicketAsync();
            return Page();
        }

        TempData["Message"] = "پاسخ با موفقیت ثبت شد.";

        return RedirectToPage("./Details", new { id });
    }


}
