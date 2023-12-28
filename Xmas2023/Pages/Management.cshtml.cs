using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xmas2023.Data;

namespace Xmas2023.Pages;

public class ManagementModel : PageModel
{
    private readonly XmasDbContext _dbContext;

    public ManagementModel(XmasDbContext context)
    {
        _dbContext = context;
    }

    public IList<DataItem> DataItems { get; set; } = new List<DataItem>();

    private IList<string> Manager = new List<string>
    {
        "admin@test.com.tw",
    };

    public async Task<IActionResult> OnGetAsync()
    {
        if (!Manager.Contains(User.Identity?.Name!))
        {
            return RedirectToPage("Index");
        }

        await Query();
        return Page();
    }

    public async Task<IActionResult> OnPostDrawStrawsAllAsync()
    {
        if (!Manager.Contains(User.Identity?.Name!))
        {
            return RedirectToPage("Index");
        }

        await _dbContext.DrawStrawsAllAsync();

        await Query();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!Manager.Contains(User.Identity?.Name!))
        {
            return RedirectToPage("Index");
        }

        var item = await _dbContext.DataItems.FindAsync(id);
        if (item != null)
        {
            _dbContext.DataItems.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        await Query();
        return Page();
    }

    private async Task Query()
    {
        DataItems = await _dbContext.DataItems
            .Include(i => i.MatchItem)
            .ToListAsync();
    }
}