using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Documents;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.FileStorage;
using Ticketing_web.Services.Replies;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Agents.Tickets;

public class DetailsModel : PageModel
{

    private readonly ITicketService _ticketService;
    private readonly ITicketReplyService _replyService;
    private readonly IFileService _fileService;
    public GetAgentTicketDetailsDto? Ticket { get; private set; }

    public string Id { get; private set; } = string.Empty;

    public string? Message { get; private set; }

    public string? ErrorMessage { get; private set; }

    [BindProperty]
    public IFormFile? Document { get; set; }
    public DetailsModel(
        ITicketService ticketService,
        ITicketReplyService replyService,
        IFileService fileService)
    {
        _ticketService = ticketService;
        _replyService = replyService;
        _fileService = fileService;
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

        DocumentDto? document = null;
        if (Document != null && Document.Length > 0)
        {
            document = await UploadFile(Document);
            if (document == null)
            {
                await LoadTicketAsync();
                return Page();
            }
            var addFile = await _replyService.AddFile(replyId, document);

            if (addFile is null || !addFile.IsSuccess)
            {
                ErrorMessage = addFile?.Error ?? "ثبت فایل ناموفق بود.";
                return Page();
            }
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

    
    
    public async Task<IActionResult> OnPostDeleteTicketReplyDocument(
        string ticketId,
        string replyId,
        string documentId
        )
    {
        if (string.IsNullOrWhiteSpace(replyId))
        {
            return NotFound();
        }
        Id = ticketId;
        if (string.IsNullOrWhiteSpace(documentId))
        {
            ErrorMessage = "خطایی در ارسال اطلاعات پیش آمده.";
            await LoadTicketAsync();
            return Page();
        }
        await LoadTicketAsync();

        if (Ticket == null)
        {
            ErrorMessage = "تیکت پیدا نشد.";
            return Page();
        }
        var removeResult = await _fileService.Delete(documentId);
        if (!removeResult.IsSuccess)
        {
            ErrorMessage = removeResult?.Error ?? "حذف فایل ناموفق بود.";
            await LoadTicketAsync();
            return Page();
        }

        var result = await _replyService.DeleteDocument(documentId);

        if (result is null || !result.IsSuccess)
        {
            ErrorMessage = result?.Error ?? "حذف فایل ناموفق بود.";

            await LoadTicketAsync();

            return Page();
        }
        TempData["Message"] = "فایل پیوست پاسخ با موفقیت حذف شد.";

        return RedirectToPage("./Details", new { id = Id });
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
        Ticket.Documents=result.Data.Documents;
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
        DocumentDto? document = null;
        if (Document != null && Document.Length > 0)
        {
            document = await UploadFile(Document);
            if (document == null)
            {
                await LoadTicketAsync();
                return Page();
            }

        }

        var result = await _replyService.ReplyTicket(
            id,
            replyContent,
            document);

        if (result == null || !result.IsSuccess)
        {
            ErrorMessage = "ثبت پاسخ ناموفق بود.";
            await LoadTicketAsync();
            return Page();
        }

        TempData["Message"] = "پاسخ با موفقیت ثبت شد.";

        return RedirectToPage("./Details", new { id });
    }
    private async Task<DocumentDto?> UploadFile(IFormFile? file)
    {
        if (file == null)
            return null;
        var fileResult = await _fileService.Upload(file);

        if (fileResult is null || !fileResult.IsSuccess || fileResult.Data == null)
        {
            ErrorMessage = fileResult?.Error ?? "آپلود فایل ناموفق بود.";
            return null;
        }
        return new DocumentDto
        {
            DocumentId = fileResult.Data.DocumentId,
            Extension = fileResult.Data.Extension
        };
    }

}
