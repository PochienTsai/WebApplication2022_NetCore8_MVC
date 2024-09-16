namespace WebApplication2022_NetCore8_MVC.Models
{
    /// <summary>
    ///  入門範例
    ///  搭配 Day1控制器的 Index4動作，產生檢視畫面
    ///  搭配 Day2控制器的 Index11動作，產生檢視畫面
    /// </summary>
    public class Class1
    {
        public string? UserName { get; set; }
        // Property 屬性
        // string?。問號（?）代表  允許輸入空值（null）
        //VS2022 這裡有些變化  https://docs.microsoft.com/zh-tw/dotnet/csharp/nullable-references
    }
}