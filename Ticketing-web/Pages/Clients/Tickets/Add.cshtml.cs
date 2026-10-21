using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Ticketing_web.EntityModels.Tickets;
using Ticketing_web.Services.Tickets;

namespace Ticketing_web.Pages.Clients.Tickets;

public class AddModel : PageModel
{

    private readonly ITicketService _ticketService;

    public AddModel(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [BindProperty]
    public CreateTicketInputDto Input { get; set; } = new();


    public void OnGet()
    {
    }
    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _ticketService.Add(new AddTicketDto()
        {
            Content = Input.Content,
            Priority = Input.Priority,
            Title = Input.Title
        });


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