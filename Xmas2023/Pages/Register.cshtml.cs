using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xmas2023.Data;
using Xmas2023.Enums;

namespace Xmas2023.Pages;

public class RegisterModel : PageModel
{
    private readonly DomainOptions _domainOptions;
    private readonly XmasDbContext _dbContext;

    [BindProperty]
    public Department Department { get; set; }

    [BindProperty]
    public string Wish { get; set; } = default!;

    public RegisterModel(DomainOptions domainOptions, XmasDbContext dbContext)
    {
        _domainOptions = domainOptions;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var checkResult = _domainOptions.CheckDate(DateTime.UtcNow);
        switch (checkResult)
        {
            case "DrawStraws":
                return RedirectToPage("DrawStraws");
            case "Register":
                break;
            default:
                return RedirectToPage("Error", new { message = checkResult });
        }

        var email = User.Identity!.Name!;

        Wish = await _dbContext.GetWishAsync(email);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var checkResult = _domainOptions.CheckDate(DateTime.UtcNow);
        switch (checkResult)
        {
            case "DrawStraws":
                return RedirectToPage("DrawStraws");
            case "Register":
                break;
            default:
                return RedirectToPage("Error", new { message = checkResult });
        }

        var email = User.Identity?.Name;
        var userName = User.FindFirst("name")?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName))
        {
            return RedirectToPage("Error");
        }

        await _dbContext.RegisterAsync(email, userName, Department, Wish);

        return Page();
    }
}
