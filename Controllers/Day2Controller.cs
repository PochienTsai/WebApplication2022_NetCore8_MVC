using Microsoft.AspNetCore.Mvc;
//*************************************
using Microsoft.AspNetCore.Http;  // .NET Core 的 IFormCollection需要搭配這個命名空間
using WebApplication2022_NetCore8_MVC.Models;
//*************************************

namespace WebApplication2022_NetCore8_MVC.Controllers
{
    public class Day2Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        // 檢視畫面上的第一列（model）與 Model，有何不同？？
        // @Model is "a property of the view" that refers to the model that was passed to the view from the controller. 
        // This property is strongly typed based on the data type specified in the @model directive at the top of the view.
        // ***  @model occurs "only once" and specifies the "data type" of the model. 
        // ***  @Model allows you to reference "the value for the model" passed to the view.

        // GET: Day2
        public IActionResult Index10()
        {   // 檢視畫面上的第一列（model）與 Model，有何不同？？

            ////傳統寫法 - 加入一筆記錄
            //Models.Class1 myClass = new Models.Class1();
            //myClass.UserName = "Hello! The World.";

            ////新寫法 - 加入一筆記錄
            //Models.Class1 myClass = new Models.Class1
            Models.Class1 myClass = new()  // VS2022的簡化寫法
            {
                // 加入一筆記錄
                UserName = "Hello! The World."
            };

            return View(myClass);
            // 產生 View（檢視畫面）時，請選 Details範本
        }


        public IActionResult Index10BeginForm()
        {   // 搭配檢視畫面中的HTML表單，透過 HTML Helper來寫表單
            return View();
            // 產生一個「空白的」檢視畫面，自動動手寫表單

            // 若要搭配 「傳統HTML表單」來寫。
            // 請參閱上一個範例 Day1控制器底下的 Index91Add動作。
        }

        public IActionResult Index10HTMLForm()
        {   // 搭配檢視畫面中的HTML表單，透過 傳統HTML表單（<form>）來寫表單
            return View();
            // 產生一個「空白的」檢視畫面，自動動手寫表單

            // 若要搭配 「傳統HTML表單」來寫。
            // 請參閱上一個範例 Day1控制器底下的 Index91Add動作。
        }

        //*******************************************************************************************************************
        public IActionResult Index10BeginFormVS2022()
        {   // 搭配檢視畫面中的HTML表單，透過 HTML Helper來寫表單
            return View();
            // 產生一個「空白的」檢視畫面，自動動手寫表單

            // 若要搭配 「傳統HTML表單」來寫。
            // 請參閱上一個範例 Day1控制器底下的 Index91Add動作。
        }



        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index11Create()
        {   // 搭配 Class1，做出新增（Create）的檢視畫面
            return View();
        }

        [ValidateAntiForgeryToken]   // 搭配檢視畫面（View）的Razor表單裡面，「@Html.AntiForgeryToken()」這句話以避免CSRF攻擊！！
                                     // https://learn.microsoft.com/zh-tw/aspnet/core/security/anti-request-forgery
        [HttpPost, ActionName("Index11Create")]    // ***重點！！刪除這一句話，會出錯！！***  完整寫法 [AcceptVerbs(HttpVerbs.Post)]
        public string Index11Create(Models.Class1 class1)
        {   // 搭配 Class1，做出新增（Create）功能
            try
            {
                if (!ModelState.IsValid)
                {   // 驗證（Validation）。預計到「第五節課」會講到
                    return "NOT ModelState.IsValid";  // 驗證失敗！
                }
                return "驗證成功！！......您剛剛輸入的 UserName是：<font color=blue>" + class1.UserName + "</font>";
            }
            catch
            {
                return "Exception：執行失敗，出現例外狀況。";
            }
        }


        //--------------------------------------------------------------------------------------------------------
        //--- VS2022產生「檢視畫面」已有變化！！ 改成 Tag Helper
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index11Create2022()
        {   // 搭配 Class1，做出新增（Create）的檢視畫面
            return View();
        }

        [ValidateAntiForgeryToken]   // 搭配檢視畫面（View）的Razor表單裡面，「@Html.AntiForgeryToken()」這句話以避免CSRF攻擊！！
                                     //  https://learn.microsoft.com/zh-tw/aspnet/core/security/anti-request-forgery
        [HttpPost, ActionName("Index11Create2022")]    // ***重點！！刪除這一句話，會出錯！！***  完整寫法 [AcceptVerbs(HttpVerbs.Post)]
        public string Index11Create2022(Models.Class1 class1)
        {   // 搭配 Class1，做出新增（Create）功能
            try
            {
                if (!ModelState.IsValid)
                {   // 驗證（Validation）。預計到「第五節課」會講到
                    return "NOT ModelState.IsValid";  // 驗證失敗！
                }
                return "驗證成功！！......您剛剛輸入的 UserName是：<font color=blue>" + class1.UserName + "</font>";
            }
            catch
            {
                return "Exception：執行失敗，出現例外狀況。";
            }
        }





        //--------------------------------------------------------------------------------------------------------
        //---- 目前還不連結資料庫。僅使用一個小類別檔（Class1.cs）來做 Model。
        //--------------------------------------------------------------------------------------------------------

        //******************************************************************
        public List<Models.Class1> myList = new List<Models.Class1>();

        // 文章說明： When To Use IEnumerable, ICollection, IList And List（文章後面有一張圖）
        // 
        //  IEnumerable -- 只用於「唯讀」的資料展示。
        //  ICollection -- 您想修改集合或關心其大小(size)。  ex:第五節課會講到
        //  IList -- 您想要修改集合，並關心集合中元素的排序和/或位置。
        //******************************************************************

        public IActionResult Index12Create()
        {   // 搭配 Class1，做出新增（Create）的檢視畫面
            return View();
        }

        [ValidateAntiForgeryToken]   // 搭配檢視畫面（View）的Razor表單裡面，「@Html.AntiForgeryToken()」這句話以避免CSRF攻擊！！
        [HttpPost]    // ***重點！！刪除這一句話，會出現錯誤！！***  完整寫法 [AcceptVerbs(HttpVerbs.Post)]
        public IActionResult Index12Create(Models.Class1 class1)
        {   // 搭配 Class1，做出新增（Create）功能
            try
            {
                if (!ModelState.IsValid)   // 驗證（Validation）。預計到「第五節課」會講到
                {
                    return View();
                }
                //畫面上，新增的字串
                myList.Add(new Models.Class1 { UserName = "您剛剛輸入 - <font color=green>" + class1.UserName + "</font>" });

                // 現在才加入的幾筆記錄
                myList.Add(new Models.Class1 { UserName = "(5). 字串 ASP.NET" });
                myList.Add(new Models.Class1 { UserName = "(6). 字串 Web Form" });
                myList.Add(new Models.Class1 { UserName = "(7). 字串 MVC" });
                myList.Add(new Models.Class1 { UserName = "(8). 字串 ASP.NET專題實務" });

                //*****************************************************
                return View("Index12List", myList);
                //--新增資料以後，要將 List<Models.Class1> 重新導向到 Index12List
                //*****************************************************
            }
            catch
            {
                return View();
            }
        }

        // 為什麼這個動作可以註解（不執行）呢？
        // 請對應 Day1底下的  Index1A與 Index1B 這兩個動作

        //public ActionResult Index12List()
        //{   // 搭配 Class1 列出、呈現（List）的檢視畫面
        //    return View(myList);
        //}


        //--------------------------------------------------------------------------------------------------------
        //---- 延續上面的範例，只修改第二個動作的「輸入值」，
        //       新版改為 IFormCollection（.NET Core 專用 / Microsoft.AspNetCore.Http.IFormCollection）
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index13Create()
        {   // 搭配 Class1，做出新增（Create）的檢視畫面
            return View();
        }

        [ValidateAntiForgeryToken]   // 搭配檢視畫面（View）的Razor表單裡面，「@Html.AntiForgeryToken()」這句話以避免CSRF攻擊！！
        [HttpPost]    // ***重點！！刪除這一句話，會出現錯誤！！***
        public IActionResult Index13Create(IFormCollection myForm)    // IFormCollection需要搭配 Microsoft.AspNetCore.Http;命名空間
        {   // 搭配 Class1，做出新增（Create）功能
            try
            {
                if (!ModelState.IsValid)   // 驗證（Validation）。預計到「第五節課」會講到
                {
                    return View();
                }
                // 這兩種寫法都可以：   
                // (1) myForm["UserName"]  或  (2) Request.Form["UserName"]            
                myList.Add(new Models.Class1 { UserName = " (1) 您剛剛輸入 - <font color=green>" + myForm["UserName"] + "</font>" });
                myList.Add(new Models.Class1 { UserName = " (2) 您剛剛輸入 - <font color=purple>" + Request.Form["UserName"] + "</font>" });

                return View("Index12List", myList);
                //--新增一筆資料以後，要重新導向到 Index12List
            }
            catch
            {
                return View();
            }
        }

        //--------------------------------------------------------------------------------------------------------
        //--- VS2022產生「檢視畫面」已有變化！！ 改成 Tag Helper
        //--------------------------------------------------------------------------------------------------------
        //     改為 IFormCollection（.NET Core 專用 / Microsoft.AspNetCore.Http.IFormCollection）
        public IActionResult Index13Create2022()
        {   // 搭配 Class1，做出新增（Create）的檢視畫面
            return View();
        }

        [ValidateAntiForgeryToken]   // 搭配檢視畫面（View）的Razor表單裡面，「@Html.AntiForgeryToken()」這句話以避免CSRF攻擊！！
        [HttpPost]    // ***重點！！刪除這一句話，會出現錯誤！！***
        public IActionResult Index13Create2022(IFormCollection myForm)    // IFormCollection需要搭配 Microsoft.AspNetCore.Http;命名空間
        {   // 搭配 Class1，做出新增（Create）功能
            try
            {
                if (!ModelState.IsValid)   // 驗證（Validation）。預計到「第五節課」會講到
                {
                    return View();
                }
                // 這兩種寫法都可以：   
                // (1) myForm["UserName"]  或  (2) Request.Form["UserName"]            
                myList.Add(new Models.Class1 { UserName = " (1) 您剛剛輸入 - <font color=green>" + myForm["UserName"] + "</font>" });
                myList.Add(new Models.Class1 { UserName = " (2) 您剛剛輸入 - <font color=purple>" + Request.Form["UserName"] + "</font>" });

                return View("Index12List", myList);
                //--新增一筆資料以後，要重新導向到 Index12List
            }
            catch
            {
                return View();
            }
        }





        //--------------------------------------------------------------------------------------------------------
        //---- 延續上面的範例，只修改第二個動作的「輸入值」，改為 string
        //--------------------------------------------------------------------------------------------------------
        public IActionResult Index14Create()
        {   // 搭配 Class1，做出新增（Create）的檢視畫面
            return View();
        }

        [ValidateAntiForgeryToken]   // 搭配檢視畫面（View）的Razor表單裡面，「@Html.AntiForgeryToken()」這句話以避免CSRF攻擊！！
        [HttpPost]    // ***重點！！刪除這一句話，會出現錯誤！！***
        public IActionResult Index14Create(string UserName)
        {   // 搭配 Class1，做出新增（Create）功能
            try
            {
                if (!ModelState.IsValid)   // 驗證（Validation）。預計到「第五節課」會講到
                {
                    return View();
                }

                myList.Add(new Models.Class1 { UserName = "您剛剛輸入 - <font color=green>" + UserName + "</font>" });

                return View("Index12List", myList);
                //--新增一筆資料以後，要重新導向到 Index12List
            }
            catch
            {
                return View();
            }
        }



        //--------------------------------------------------------------------------------------------------------
        //--- C# Lambda 用法
        //--------------------------------------------------------------------------------------------------------
        delegate int del1(int i);

        public int Index15Lambda()
        {   // C# Lambda 用法       

            del1 myDelegate = (x => x * x);   // 計算 平方值
            int j = myDelegate(5);

            return j;
        }


        // 陣列的 .Any()函數。  找出（字串）陣列裡面，是否有「3」？
        // LINQ寫法：  字串陣列.Any(x => "關鍵字".Contains(x))
        public string Index16Lambda()
        {   // C# Lambda 用法       
            string str = "1,11,13,15,33,51";
            string[] strArray = str.Split(',');

            // 搜尋「3」 結果為 False
            //return strArray.Any(s => "3".Contains(s)).ToString();
            return strArray.Any("3".Contains).ToString();  // 這樣的寫法跟上一列程式相同。 // https://stackoverflow.com/questions/4191108/c-sharp-check-if-string-contains-any-matches-in-a-string-array

            // ***重點！！ ***字串陣列裡面的「字」，是否符合某一個字？ true or false
            // LINQ寫法：  字串陣列.Any(x => "關鍵字".Contains(x))  或是  字串陣列.Any("關鍵字".Contains)
        }


        //==============================================
        // 以下兩個範例的結果相同，只是作法有差異
        //==============================================
        public int Index17Lambda()
        {   // C# Lambda 用法       
            string[] words = { "9-blueberry", "3-ABC", "6-cherry", "5-apple", "6-banana", "10-WaterMelon" };

            // 計算陣列裡面，每個單字的 "字數"。選出長度最小的。
            int shortestWordLength = words.Min(w => w.Length);
            //**********************************************************

            return shortestWordLength;   // 答案是 5。也就是 "3-ABC"，陣列的第二個元素。
        }


        public int Index18Lambda()
        {   // C# 改用查詢語法       
            string[] words = { "9-blueberry", "3-ABC", "6-cherry", "5-apple", "6-banana", "10-WaterMelon" };

            var query = from w in words
                        select w.Length;
            // query裡面的結果是 11, 5, 8, 7, 8, 12（陣列裡面 每一個元素 的 文字長度）

            // 用 .Min()方法找出 query裡面的最小值，答案是5。
            int shortestWordLength2 = query.Min();

            return shortestWordLength2;   // 答案是 5。也就是 "3-ABC"，陣列的第二個元素。
        }


    }
}
