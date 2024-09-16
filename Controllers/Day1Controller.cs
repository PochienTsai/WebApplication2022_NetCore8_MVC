using Microsoft.AspNetCore.Mvc;
//***********************************************
using WebApplication2022_NetCore8_MVC.Models;  // 請自己手動加入這個命名空間。
namespace WebApplication2022_NetCore8_MVC.Controllers
{
    public class Day1Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
            // 產生一個 檢視（View）畫面，套用「Empty範本」
            // 產生檢視（View）的時候，不使用任何「版面配置頁」（類似Web Form的 MasterPage 母版頁面）
        }
        public IActionResult Index1A()
        {   // *** 不要  產生檢視畫面！ *** // 請自己輸入網址URL來觀看結果。
            return View("IndexHello");
            // 在View()裡面強制寫上檢視畫面的「檔名」，
            // 當您瀏覽 http://您的網站/Day1/Index1A 時，會自動跳去 IndexHello檢視畫面（注意！沒有撰寫 IndexHello動作）
        }
        public IActionResult Index1B()
        {   // *** 不要  產生檢視畫面！ *** // 請自己輸入網址URL來觀看結果。
            return View("~/Views/Day1/IndexHello.cshtml");
            // 在View()裡面強制寫上檢視畫面的「完整路徑 與 檔名」，
            // 當您瀏覽 http://您的網站/Day1/Index1B 時，會自動跳去 IndexHello檢視畫面（注意！沒有撰寫 IndexHello動作）
        }
        // 重點！傳回值的資料型態改為 字串（string）
        public string Index1C()
        {   // *** 不要  產生檢視畫面！ *** // 請自己輸入網址URL來觀看結果。
            return "傳回一句話，字串 -- <h3>string</h3>";
        }
        public IActionResult Index1D()
        {   // *** 不要  產生檢視畫面！ *** // 請自己輸入網址URL來觀看結果。
            //return Content("用Content傳回一句話，<h3>Content()</h3>");
            // 另一種寫法
            return Content("用Content傳回一句話，<h3>Content()</h3>", "text/Html", System.Text.Encoding.UTF8);  // 最後需要 System.Text 命名空間
            // 參考資料  ActionResult  https://msdn.microsoft.com/zh-tw/library/system.web.mvc.actionresult(v=vs.118).aspx
            //                                        https://learn.microsoft.com/zh-tw/aspnet/core/web-api/action-return-types?view=aspnetcore-7.0
            //            ContentResult
            //            EmptyResult
            //            FileResult
            //            HttpUnauthorizedResult
            //            JavaScriptResult
            //            JsonResult
            //            RedirectResult
            //            RedirectToRouteResult
            //            ViewResultBase
        }
        public IActionResult Index1E()
        {   // *** 不要  產生檢視畫面！ *** // 請自己輸入網址URL來觀看結果。
            return Redirect("https://www.yahoo.com.tw/");   // 輸入URL網址，連到其他網站
        }
        //--- 補充教材 ------------------------------------------------------------------------------------------(start)
        // 如果找不到動作（Action）或是輸入錯誤的動作名稱，一律跳回首頁
        // Controller的 HandleUnknownAction方法為 virtual，所以可用override覆寫。https://msdn.microsoft.com/zh-tw/library/dd492730(v=vs.118).aspx
        //protected override void HandleUnknownAction(string actionName)
        //{
        //    Response.Redirect("http://公司首頁(網址)/");    // 自訂結果 -- 找不到動作就跳回首頁
        //    base.HandleUnknownAction(actionName);
        //}
        //--- 補充教材 ------------------------------------------------------------------------------------------(end)
        //--------------------------------------------------------------------------------------------------------
        //  版面配置頁  ( @RenderBody() )
        //  https://learn.microsoft.com/zh-tw/aspnet/web-pages/overview/ui-layouts-and-themes/3-creating-a-consistent-look?source=recommendations
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index2()     //  "第一天" 的 範例
        {
            return View();
            // 產生檢視（View）的時候，搭配「版面配置頁」（類似Web Form的 MasterPage 母版頁面）
            // 自己設定的「版面配置頁」，檔案位於 ~/Views/Shared/_LayoutPage1.cshtml
            // 重點！！ @RenderBody()
            // 完成後，可搭配 Index8 一起學習。
        }
        public IActionResult Index2A()
        {
            return View();
            // 產生檢視（View）的時候，搭配「版面配置頁」
            // 預設的「版面配置頁」，如果不選任何版面配置頁，就自動採用 ~/Views/_ViewStart.cshtml
            // 重點！！ @RenderBody()
        }
        //--------------------------------------------------------------------------------------------------------
        //---- 版面配置頁  ( @RenderSection )
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index8()
        {   // 延續上一個範例 Index2，需要搭配另一個「版面配置頁」。檔名_LayoutPage2.cshtml
            // 產生檢視畫面時，「範本」請選 Empty（沒有模型）
            return View();
            // 產生檢視（View）的時候，搭配「版面配置頁」
            // 自己設定的「版面配置頁」，檔案位於 ~/Views/Shared/_LayoutPage2.cshtml
            // 重點！！ @RenderSection()
            // https://learn.microsoft.com/zh-tw/aspnet/core/mvc/views/layout
            // https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.razor.razorpage.rendersection
        }
        //--------------------------------------------------------------------------------------------------------
        //---- ViewBag、ViewData
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index3()
        {
            ViewData["A"] = "(1). 字串A。我是ViewData[]";
            ViewBag.B = "(2). 字串B。我是ViewBag";
            TempData["C"] = "(3). 字串C。我是TempData[]";
            return View();
            // 產生檢視（View）的時候，不使用任何「版面配置頁」
        }
        // 如果ViewData & ViewBag 相同命名，會出現什麼情況？？
        // 例如：ViewData["XYZ"] & ViewBag.XYZ  ？？？？
        public IActionResult Index3A()
        {
            ViewData["XYZ"] = "** 您好：我是ViewData[] **";
            ViewBag.XYZ = "抱歉！改了內容，我是ViewBag。";
            return View();
            // 產生檢視（View）的時候，不使用任何「版面配置頁」
            // 補充教材：https://www.c-sharpcorner.com/UploadFile/db2972/datatable-in-viewdata-sample-in-mvc-day-3/
            // DataTable，透過 ViewBag傳遞給 檢視畫面使用
        }
        //--------------------------------------------------------------------------------------------------------
        //-- 搭配第一個類別檔 (~/Models/Class1.cs) 當作Model，但沒有連結資料庫。
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index4()
        {   // 搭配類別檔 ~/Models/Class1.cs
            //新寫法
            //var uname = new Models.Class1 { UserName = "(4). 字串D" };
            //或是寫成 Models.Class1 uname = new Models.Class1 { UserName = "(4). 字串D" };
            //傳統寫法
            var uname = new Models.Class1();
            uname.UserName = "(4). 字串D。我躲在Class1類別 的 UserName 屬性。";
            return View(uname);
            // 產生檢視（View）的時候，不使用任何「版面配置頁」
            // 請使用「Details」範本。只傳遞「一筆」數據而已。
        }
        public IActionResult Index5()
        {   // 搭配一個簡單的類別檔，當作 Model與檢視畫面（View）互動
            List<Models.Class1> myList = new List<Models.Class1>
            {
                new Models.Class1 { UserName = "(5). 字串 ASP.NET。" },
                new Models.Class1 { UserName = "(6). 字串 Web Form。" },
                new Models.Class1 { UserName = "(7). 字串 MVC。" },
                new Models.Class1 { UserName = "(8). 字串 ASP.NET專題實務。" }
            };
            List<Class1> myList2 = new List<Class1>();
            myList2.Add(new Class1 { UserName = "(5). 字串 ASP.NET。" });
            myList2.Add(new Class1 { UserName = "(9). 字串 ASP.NET。" });
            myList2.Add(new Class1 { UserName = "(10). 字串 Web Form。" });
            myList2.Add(new Class1 { UserName = "(11). 字串 MVC。" });
            myList2.Add(new Class1 { UserName = "(12). 字串 ASP.NET專題實務。" });
            // //上面是簡化的寫法。也可以寫成這樣（舊寫法）
            //List<Models.Class1> myList = new List<Models.Class1>();
            //    myList.Add(new Models.Class1 { UserName = "(5). 字串 ASP.NET。" });
            //    myList.Add(new Models.Class1 { UserName = "(6). 字串 Web Form。" });
            //    myList.Add(new Models.Class1 { UserName = "(7). 字串 MVC。" });
            //    myList.Add(new Models.Class1 { UserName = "(8). 字串 ASP.NET專題實務。" });
            return View(myList);
            // 產生檢視（View）的時候，不使用任何「版面配置頁」
            // 請使用「List」範本。因為需要傳遞「多筆」數據。
            // 文章說明： When To Use IEnumerable, ICollection, IList And List（文章後面有一張圖）
            // http://www.claudiobernasconi.ch/2013/07/22/when-to-use-ienumerable-icollection-ilist-and-list/
            //  IEnumerable -- 只用於「唯讀」的資料展示。
            //  ICollection -- 您想修改集合或關心其大小(size)。
            //  IList -- 您想要修改集合，並關心集合中元素的排序和/或位置。
        }
        //--------------------------------------------------------------------------------------------------------
        //---- 以下範例，將練習「檢視畫面」的 Razor。  控制器不需額外的程式碼。
        //       https://learn.microsoft.com/zh-tw/aspnet/web-pages/overview/getting-started/introducing-razor-syntax-c
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index6()
        {   // 產生檢視畫面時，「範本」請選 Empty（沒有模型）
            return View();
            // 產生檢視（View）的時候，不使用任何「版面配置頁」
        }
        public IActionResult Index7()
        {   // 產生檢視畫面時，「範本」請選 Empty（沒有模型）
            return View();
            // 產生檢視（View）的時候，不使用任何「版面配置頁」
        }
        public IActionResult Index7for()
        {   // 學習 for 迴圈
            return View();
            // 產生檢視（View）的時候，不使用任何「版面配置頁」（類似Web Form的 MasterPage 母版頁面）
        }
        public IActionResult Index7Request()
        {   // xxxxx 學習 Context.Request  xxxxx     ** .NET Core已經沒有這種寫法 **
            // 改成這樣的寫法 - string userAgent = Context.Request.Headers["User-Agent"];
            return View();
            // 產生檢視（View）的時候，不使用任何「版面配置頁」（類似Web Form的 MasterPage 母版頁面）
        }
        //--------------------------------------------------------------------------------------------------------
        //----  HTML表單（<Form>）搭配 Razor、檢視畫面
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index90IsPost()
        {   // 類似Web Form的「Page.IsPostBack」，搭配檢視畫面中的傳統HTML表單
            return View();
            // 產生檢視（View）的時候，不使用任何「版面配置頁」（類似Web Form的 MasterPage 母版頁面）
        }
        //// 範例可運作，但已棄置不用。
        //// 請看後續的範例（Day2控制器底下，動作 Index10BeginForm()）
        //public IActionResult Index91Add()
        //{   // 搭配檢視畫面中的傳統HTML表單
        //    return View();
        //}
        public IActionResult Index92Array()
        {   // 陣列
            return View();
        }
        public IActionResult Index93Dictionary()
        {   // 資料字典 (Dictionary)
            return View();
        }
    }
}