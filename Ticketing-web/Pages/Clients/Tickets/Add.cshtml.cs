using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Ticketing_web.EntityModels.Documents;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.FileStorage;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Clients.Tickets;

public class AddModel : PageModel
{

    private readonly ITicketService _ticketService;
    private readonly IFileService _fileService;

    public AddModel(
        ITicketService ticketService,
        IFileService fileService)
    {
        _ticketService = ticketService;
        _fileService = fileService;
    }

    [BindProperty]
    public CreateTicketInputDto Input { get; set; } = new();

    [BindProperty]
    public IFormFile? Document { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
            return Page();
        DocumentDto? docResult = null;
        if (Document is { Length: > 0 })
        {
            var docInfo = await _fileService.Upload(Document);
            if (docInfo?.IsSuccess == true && docInfo.Data != null)
            {
                docResult = new DocumentDto
                {
                    DocumentId = docInfo.Data.DocumentId,
                    Extension = docInfo.Data.Extension
                };
            }
            else
            {
                ModelState.AddModelError(
                    nameof(Document),
                    "آپلود فایل با خطا مواجه شد.");

                return Page();
            }
        }
        var result = await _ticketService.Add(new AddTicketDto
        {
            Content = Input.Content,
            Priority = Input.Priority,
            Title = Input.Title,
            Document = docResult
        });
        if (result?.IsSuccess != true)
        {
            ModelState.AddModelError(
                string.Empty,
                "ایجاد تیکت با خطا مواجه شد.");

            return Page();
        }
        return Redirect("/Clients/Tickets");
    }
}


public class CreateTicketInputDto
{
    [Required(ErrorMessage = "وارد کردن عنوان الزامی است.")]
    [StringLength(200, ErrorMessage = "عنوان نباید بیشتر از ۲۰۰ کاراکتر باشد.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "انتخاب اولویت الزامی است.")]
    [Range(1, 3, ErrorMessage = "اولویت انتخاب‌شده معتبر نیست.")]
    public Priority Priority { get; set; }

    [Required(ErrorMessage = "شرح درخواست الزامی است.")]
    public string Content { get; set; } = string.Empty;
}