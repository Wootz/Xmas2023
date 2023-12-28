using NUnit.Framework;
using Xmas2023;

namespace Xmas2023.Test
{
    [TestFixture]
    public class DomainOptionsExtensionsTests
    {
        [Theory]
        [TestCase("2023-10-15+08:00", "報名未開始")]
        [TestCase("2023-11-05+08:00", "Register")]
        [TestCase("2023-11-10 08:00+08:00", "尚未開始抽籤")]
        [TestCase("2023-11-11+08:00", "DrawStraws")]
        public void CheckDate_ReturnsTrue_WhenDateIsWithinRange(string date, string r)
        {
            // Arrange
            var options = new DomainOptions
            {
                RegisterStartDate = DateTime.Parse("2023-11-01+08:00"),
                RegisterEndDate = DateTime.Parse("2023-11-10+08:00"),
                DrawStrawsStartDate = DateTime.Parse("2023-11-10 09:00+08:00"),
            };

            // Act
            var dateToCheck = DateTime.Parse(date).ToUniversalTime();
            var result = options.CheckDate(dateToCheck);

            // Assert
            Assert.IsTrue(result == r);
        }
    }
}