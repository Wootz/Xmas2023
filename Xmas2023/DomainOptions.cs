namespace Xmas2023;

public class DomainOptions
{
    /// <summary>
    /// 報名起始時間
    /// </summary>
    public DateTime? RegisterStartDate { get; set; }

    /// <summary>
    /// 報名結束時間
    /// </summary>
    public DateTime? RegisterEndDate { get; set; }

    /// <summary>
    /// 抽籤起始時間
    /// </summary>
    public DateTime? DrawStrawsStartDate { get; set; }
}

public static class DomainOptionsExtensions
{
    /// <summary>
    /// 檢查是否在報名期間 是：回傳 "Register"
    /// 檢查是否在抽籤期間 是：回傳 "DrawStraws"
    /// 在報名結束時間 至 抽籤起始時間，回傳 "未開放抽籤"
    /// 在抽籤結束時間之後，回傳 ""
    /// </summary>
    /// <param name="options">The domain options to check against.</param>
    /// <param name="utc">The date to check, in UTC.</param>
    /// <returns></returns>
    public static string CheckDate(this DomainOptions options, DateTime utc)
    {
        if (options.RegisterStartDate.HasValue && utc < options.RegisterStartDate.Value.ToUniversalTime())
        {
            return "報名未開始";
        }

        if (options.RegisterEndDate.HasValue && utc < options.RegisterEndDate.Value.ToUniversalTime())
        {
            return "Register";
        }

        if (options.DrawStrawsStartDate.HasValue && utc < options.DrawStrawsStartDate.Value.ToUniversalTime())
        {
            return "尚未開始抽籤";
        }

        return "DrawStraws";
    }
}
