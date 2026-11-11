using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using Ticketing_web.EntityModels.Documents;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.EntityModels.Users;
using Ticketing_web.Services.FileStorage;
using Ticketing_web.Services.Replies;
using Ticketing_web.Services.Tickets;
using Ticketing_web.Services.Users;

namespace Ticketing_web.Pages.Admin.Tickets;

public class DetailsModel : PageModel
{
    private readonly ITicketService _ticketService;
    private readonly IUserService _userService;
    private readonly ITicketReplyService _replyService;
    private readonly IFileService _fileService;

    public DetailsModel(
        ITicketService ticketService,
        IUserService userService,
        ITicketReplyService replyService,
        IFileService fileService)
    {
        _ticketService = ticketService;
        _userService = userService;
        _replyService = replyService;
        _fileService = fileService;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    public GetTicketDetailsDto? Ticket { get; private set; }

    [TempData]
    public string? Message { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public string AssignedUserId { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "شناسه پاسخ الزامی است.")]
    public string ReplyId { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "متن پاسخ الزامی است.")]
    public string EditedContent { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? Document { get; set; }

    public List<AgentOptionDto> Agents { get; private set; } = new();
    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return NotFound();
        await LoadTicketAsync();
        if (Ticket == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAssignAsync()
    {
        if (string.IsNullOrWhiteSpace(AssignedUserId))
        {
            ErrorMessage = "ابتدا یک کارشناس را انتخاب کنید.";
            return RedirectToPage(new { id = Id });
        }

        var ticket = await GetTicketOrFailAsync();

        if (ticket == null)
        {
            ErrorMessage = "تیکت پیدا نشد.";
            return RedirectToPage(new { id = Id });
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            ErrorMessage = "تیکت بسته‌شده قابل ارجاع نیست.";
            return RedirectToPage(new { id = Id });
        }

        var result = await _ticketService.AssignTicketAsync(
            Id,
            AssignedUserId);

        if (!result.IsSuccess)
        {
            ErrorMessage = "ارجاع تیکت انجام نشد.";
            return RedirectToPage(new { id = Id });
        }

        Message = "تیکت با موفقیت ارجاع شد.";

        return RedirectToPage(new { id = Id });
    }

    private async Task<GetTicketDetailsDto?> GetTicketOrFailAsync()
    {
        await LoadTicketAsync();
        return Ticket;
    }
 

    private async Task LoadTicketAsync()
    {
        var ticketResult = await _ticketService.GetDetailsForAdminAsync(Id);

        var agents = await _userService.GetAllAgents();

        if(agents.IsSuccess && agents.Data!=null)
        {
            Agents=agents.Data;
        }

        if(ticketResult.IsSuccess && ticketResult.Data != null)
        {
            Ticket = new GetTicketDetailsDto()
            {
                AskedBy = ticketResult.Data.AskedBy,
                AssignmentHistory = ticketResult.Data.AssignmentHistory,
                Content = ticketResult.Data.Content,
                CreatedAt = ticketResult.Data.CreatedAt,
                Number = ticketResult.Data.Number,
                OwnerOrgan = ticketResult.Data.OwnerOrgan,
                Priority = ticketResult.Data.Priority,
                Replies = ticketResult.Data.Replies,
                Status = ticketResult.Data.Status,
                Title = ticketResult.Data.Title,
                Documents=ticketResult.Data.Documents,
            };
        }
    }



    public async Task<IActionResult> OnPostApprove(
        string id,
        string replyId
        )
    {
        if (string.IsNullOrWhiteSpace(replyId))
        {
            return NotFound();
        }
        Id = id;
        await LoadTicketAsync();
        if (Ticket == null)
        {
            ErrorMessage = "تیکت پیدا نشد.";
            return Page();
        }

        var result = await _replyService.Approve(replyId);
        if (result == null || !result.IsSuccess)
        {
            ErrorMessage = "تایید پاسخ ناموفق بود.";
            await LoadTicketAsync();
            return Page();
        }

        TempData["Message"] = "پاسخ با موفقیت تایید شد.";

        return RedirectToPage("./Details", new { Id });
    }



    public async Task<IActionResult> OnPostEditReply(
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
       
        DocumentDto? document = null;
        if (Document != null && Document.Length > 0)
        {
            document = await UploadFile(Document);
            if (document == null)
            {
                await LoadTicketAsync();
                return Page();
            }
            var addFile = await _replyService.AddFile(ReplyId, document);

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
        string documentId)
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
