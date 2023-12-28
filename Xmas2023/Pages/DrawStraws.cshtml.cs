using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xmas2023.Data;

namespace Xmas2023.Pages;

public class DrawStrawsModel : PageModel
{
    private readonly DomainOptions _domainOptions;
    private readonly XmasDbContext _dbContext;

    public string? Uid { get; set; }

    public string? Wish { get; set; }

    public DrawStrawsModel(DomainOptions domainOptions, XmasDbContext dbContext)
    {
        _domainOptions = domainOptions;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var checkResult = _domainOptions.CheckDate(DateTime.UtcNow);
        switch (checkResult)
        {
            case "Register":
                return RedirectToPage("Register");
            case "DrawStraws":
                break;
            default:
                return RedirectToPage("Error", new { message = checkResult });
        }

        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("Error");
        }

        var item = await _dbContext.DataItems
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Email == email);
        if (item == null)
        {
            return RedirectToPage("Error", new { message = "你忘記報名了" });
        }

        var matched = await _dbContext.GetMatchedItemAsync(email);

        Uid = matched?.Uid;
        Wish = matched?.Wish;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var checkResult = _domainOptions.CheckDate(DateTime.UtcNow);
        switch (checkResult)
        {
            case "Register":
                return RedirectToPage("Register");
            case "DrawStraws":
                break;
            default:
                return RedirectToPage("Error", new { message = checkResult });
        }

        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("Error");
        }

        await _dbContext.DrawStrawsAsync(email);
        var matched = await _dbContext.GetMatchedItemAsync(email);

        Uid = matched?.Uid;
        Wish = matched?.Wish;

        return Page();
    }
}
