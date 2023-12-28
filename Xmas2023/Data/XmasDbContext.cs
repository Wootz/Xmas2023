using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Xmas2023.Enums;

namespace Xmas2023.Data;

public class XmasDbContext : DbContext
{
    public DbSet<DataItem> DataItems { get; set; }

    public XmasDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataItem>()
            .HasOne(i => i.MatchItem)
            .WithOne(i => i.MatchFor)
            .HasForeignKey<DataItem>(nameof(DataItem.MatchedId))
            .OnDelete(DeleteBehavior.Restrict);
    }

    public async Task RegisterAsync(string email, string userName, Department department, string wish)
    {
        var uid = GenerateUid(department);
        while (await DataItems.AsNoTracking().AnyAsync(i => i.Uid == uid))
        {
            uid = GenerateUid(department);
        }

        var item = new DataItem
        {
            Email = email,
            UserName = userName,
            Department = department,
            Wish = wish,
            Uid = uid,
            CreatedDate = DateTimeOffset.UtcNow,
        };

        await DataItems.AddAsync(item);
        await SaveChangesAsync();
    }

    public async Task<string> GetWishAsync(string email)
    {
        return await DataItems
            .AsNoTracking()
            .Where(i => i.Email == email)
            .Select(i => i.Wish)
            .SingleOrDefaultAsync() ?? "";
    }

    public Task<DataItem?> GetMatchedItemAsync(string email)
    {
        return DataItems
            .AsNoTracking()
            .Where(i => i.Email == email)
            .Select(i => i.MatchItem)
            .SingleOrDefaultAsync();
    }

    public async Task DrawStrawsAllAsync()
    {
        var sql = @"
DECLARE @email NVARCHAR(200)
DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT Email FROM DataItems

OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @email
WHILE @@FETCH_STATUS = 0
BEGIN 
    UPDATE DataItems
        SET MatchedId = (
            SELECT TOP 1 D1.Id
            FROM DataItems D1
            WHERE D1.Id <> DataItems.Id AND D1.Id NOT IN (SELECT D2.MatchedId FROM DataItems D2 WHERE D2.MatchedId IS NOT NULL)
            ORDER BY NEWID()
        )
    WHERE MatchedId IS NULL AND Email = @email

    FETCH NEXT FROM MY_CURSOR INTO @email
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR";

        await Database.ExecuteSqlRawAsync(sql);
    }

    public async Task DrawStrawsAsync(string email)
    {
        var sql = @"
UPDATE DataItems
    SET MatchedId = (
        SELECT TOP 1 D1.Id
        FROM DataItems D1
        WHERE D1.Id <> DataItems.Id AND D1.Id NOT IN (SELECT D2.MatchedId FROM DataItems D2 WHERE D2.MatchedId IS NOT NULL)
        ORDER BY NEWID()
    )
WHERE MatchedId IS NULL AND Email = @p0";

        await Database.ExecuteSqlRawAsync(sql, email);
    }

    private string GenerateUid(Department department)
    {
        var random = new Random(Guid.NewGuid().GetHashCode());
        var uid = (int)department switch
        {
            >= 10 and < 20 => random.Next(1, 99).ToString("D3") + random.Next(0, 999).ToString("D3"),
            >= 20 and < 30 => random.Next(100, 199).ToString("D3") + random.Next(0, 999).ToString("D3"),
            _ => random.Next(200, 299).ToString("D3") + random.Next(0, 999).ToString("D3"),
        };

        return uid;
    }
}

[Index(nameof(Uid), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class DataItem
{
    [Key]
    public int Id { get; set; }

    public int? MatchedId { get; set; }

    [MaxLength(200)]
    public string Email { get; set; } = default!;

    [MaxLength(200)]
    [Display(Name = "姓名")]
    public string UserName { get; set; } = default!;

    [Display(Name = "部門")]
    public Department Department { get; set; }

    [Display(Name = "願望")]
    public string Wish { get; set; } = default!;

    [MaxLength(200)]
    public string Uid { get; set; } = default!;

    public DateTimeOffset CreatedDate { get; set; }

    public DataItem? MatchItem { get; set; }

    public DataItem? MatchFor { get; set; }
}