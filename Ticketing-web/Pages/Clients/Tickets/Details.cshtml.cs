using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ticketing_web.EntityModels.Documents;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.FileStorage;
using Ticketing_web.Services.Replies;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Clients.Tickets;

public class DetailsModel : PageModel
{
    private readonly ITicketService _ticketService;
    private readonly ITicketReplyService _replyService;
    private readonly IFileService _fileService;

    public DetailsModel(
        ITicketService ticketService,
        ITicketReplyService replyService,
        IFileService fileService)
    {
        _ticketService = ticketService;
        _replyService = replyService;
        _fileService = fileService;
    }
    public string Id { get; private set; } = string.Empty;

    public GetClientTicketDetailsDto? Ticket { get; private set; }
    [BindProperty]
    public string ReplyContent { get; set; } = string.Empty;

    [BindProperty]
    public string ReplyId { get; set; } = string.Empty;

    [BindProperty]
    public string EditedReplyContent { get; set; } = string.Empty;

    [BindProperty]
    public string TicketContent { get; set; } = default!;

    [BindProperty]
    public string Title { get; set; } = default!;

    [BindProperty]
    public Priority Priority { get; set; } = default!;



    [BindProperty]
    public IFormFile? Document { get; set; }
    [BindProperty]
    public IFormFile? TicketDocument { get; set; }

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
            Id,
            ReplyContent,
            document
            );

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

        if (!IsValidContent(EditedReplyContent))
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
            EditedReplyContent.Trim());

        if (result is null || !result.IsSuccess)
        {
            ErrorMessage = result?.Error ?? "ویرایش پاسخ ناموفق بود.";
            return Page();
        }

        TempData["Message"] = "پاسخ با موفقیت ویرایش شد.";

        return RedirectToPage("./Details", new { id = Id });
    }

    public async Task<IActionResult> OnPostDeleteTicketDocument(
        string documentId,
        string id)
    {
        Id = id;
        var removeResult = await _fileService.Delete(documentId);
        if (!removeResult.IsSuccess)
        {
            ErrorMessage = removeResult?.Error ?? "حذف فایل ناموفق بود.";
            await LoadTicketAsync();
            return Page();
        }

        var result = await _ticketService.DeleteDocument(id, documentId);

        if (result is null || !result.IsSuccess)
        {
            ErrorMessage = result?.Error ?? "ثبت فایل ناموفق بود.";
            return Page();
        }

        TempData["Message"] = " حذف فایل با موفقیت انجام شد ";

        return RedirectToPage("./Details", new { id = Id });
    }

    public async Task<IActionResult> OnPostEditTicket(
        string content,
        string title,
        Priority priority,string id)
    {

        Id = id;
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        Id = id;
        TicketContent=content;
        Title = title;
        Priority= priority; 


        if (!IsValidContent(TicketContent))
        {
            ErrorMessage = "متن  باید بین ۲ تا ۵۰۰۰ کاراکتر باشد.";
            await LoadTicketAsync();
            return Page();
        }
        if (!IsValidContent(Title))
        {
            ErrorMessage = "عنوان نباید خالی باشد";
            await LoadTicketAsync();
            return Page();
        }
        var result = await _ticketService.EditTicket(new EditTicketDto()
        {
           Priority = priority,
           Title = Title,
           Content = TicketContent,
           TicketId=Id
        });

        if(!result.IsSuccess)
        {
            ErrorMessage ??= result?.Error ?? "دریافت اطلاعات تیکت ناموفق بود.";
            await LoadTicketAsync();
            return Page();
        }

        TempData["Message"] = "تیکت با موفقیت ویرایش شد.";

        return RedirectToPage("./Details", new { id = Id });
    }


    public async Task<IActionResult> OnPostSendFile(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        Id = id;

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

        var result = await _ticketService.AddFile(id, document);
        if (result is null || !result.IsSuccess)
        {
            ErrorMessage = result?.Error ?? "ثبت فایل ناموفق بود.";
            return Page();
        }

        TempData["Message"] = "فایل با موفقیت ثبت شد.";

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
