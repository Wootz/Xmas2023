using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xmas2023.Data;

namespace Xmas2023.Test;

[TestFixture]
public class DbContextTests
{
    private XmasDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddDbContext<XmasDbContext>(options =>
        {
            options.UseSqlServer("Server=127.0.0.1;Database=Xmas2023;User Id=sa;Password=Aa123456;MultipleActiveResultSets=true;TrustServerCertificate=True");
        });

        var serviceProvider = services.BuildServiceProvider();
        _dbContext = serviceProvider.GetRequiredService<XmasDbContext>();
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

    [Test]
    [Order(1)]
    public async Task RegisterTest()
    {
        // Arrange
        var count = 500;

        // Act
        for (var i = 0; i < count; i++)
        {
            await _dbContext.RegisterAsync(
                email: $"user{i}@test.com", 
                userName: $"user{i}",
                department: 0,
                wish: "123456"
            );
        }

        // Assert
        Assert.Pass();
    }

    [Test]
    [Order(2)]
    public async Task DrawStrawsTest()
    {
        // Arrange
        var count = 50;
        for (var i = 0; i < count; i++)
        {
            await _dbContext.RegisterAsync(
                email: $"user{i}@test.com", 
                userName: $"user{i}",
                department: 0,
                wish: "123456"
            );
        }

        // Act
        for (var i = 0; i < count; i++)
        {
            await _dbContext.DrawStrawsAsync($"user{i}@test.com");
        }

        // Assert
        var error = _dbContext.DataItems
            .AsNoTracking()
            .Any(i => i.Id == i.MatchedId || i.MatchedId == null);

        Assert.IsFalse(error);
    }

    [Test]
    [Order(3)]
    public async Task DrawStrawsAllTest()
    {
        // Arrange        
        var count = 50;
        for (var i = 0; i < count; i++)
        {
            await _dbContext.RegisterAsync(
                email: $"user{i}@test.com", 
                userName: $"user{i}",
                department: 0,
                wish: "123456"
            );
        }

        // Act
        await _dbContext.DrawStrawsAllAsync();

        // Assert
        var error = _dbContext.DataItems
            .AsNoTracking()
            .Any(i => i.Id == i.MatchedId || i.MatchedId == null);

        Assert.IsFalse(error);
    }
}