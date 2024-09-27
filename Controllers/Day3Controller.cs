using Microsoft.AspNetCore.Mvc;
//************************************************************
using WebApplication2022_NetCore8_MVC.Models; // 請自己手動加入這個命名空間。
using Microsoft.EntityFrameworkCore; // Async「非同步」會用到的命名空間
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient; // 請自己手動安裝NuGet

namespace WebApplication2022_NetCore8_MVC.Controllers
{
    //ADO.net建議使用DataReader(效能快,省資源）
    //1. 查詢．撈取紀錄 ：配合Select 的SQL指令(.ExecuteReader())
    //2. 新增，刪除，修改：配合Insert/Delete/Update的SQL指令(.ExecuteNonQuery())
    public class Day3Controller : Controller
    {
        //*************************************   連結 MVC_UserDB 資料庫  ********************************* (start)

        #region

        private readonly MvcUserDbContext _db; // .Net Core搭配DI，所以後面部需要使用 new一個實例。
        // 舊版寫法 private readonly MvcUserDbContext _db = new MvcUserDbContext _db();
        // 如果沒寫上方的命名空間 --「專案名稱.Models」，就得寫成下面這樣，加註「Models.」字樣。
        // private Models.MvcUserDbContext _db;

        #endregion

        //*************************************   連結 MVC_UserDB 資料庫  ********************************* (end)
        private readonly ILogger<Day3Controller> _logger;
        private readonly string _connectionString; //ADO.NET的連線字串

        public Day3Controller(ILogger<Day3Controller> logger, MvcUserDbContext context, string connectionString)
        {
            //                                                                                    ****************************（自己動手加上）
            _logger = logger;
            _db = context; //*****************************（自己動手加上）
            _connectionString = connectionString;
            // https://blog.givemin5.com/asp-net-mvc-core-tao-bi-hen-jiu-di-yi-lai-zhu-ru-dependency-injection/
        }

        public IActionResult Index()
        {
            return View(); // 空白的畫面與動作。 無作用。
        }

        //=================================================
        //== 以上都是預設。原本 Day3控制器就有的動作
        //=================================================
        //===================================
        //== 新增 ==
        //===================================
        //*** 重點！！本範例 加入檢視畫面時，"沒有" 勾選「參考指令碼程式庫」，所以沒有產生表單驗證
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")] // 把下面的動作名稱，改成 CreateConfirm 試試看？
        [ValidateAntiForgeryToken] // 避免CSRF攻擊。在Form (TagHelper) - 如果有寫 action屬性、或是Method = Get就會啟動此功能。設定為true。
        // https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.taghelpers.formtaghelper
        public IActionResult Create(UserTable _userTable)
        {
            if ((_userTable != null) &&
                (ModelState.IsValid)) // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {
                // 第一種方法   或 參考 https://medium.com/better-programming/beginners-guide-to-entity-framework-d862c9aaec4
                _db.UserTables.Add(_userTable);
                _db.SaveChanges();
                //    // ADO .NET 的寫法
                //    string connectionString = _connectionString; // 請替換為您的 MySQL 連接字串
                //    using (MySqlConnection conn = new MySqlConnection(connectionString))
                //    {
                //        conn.Open();
                //        string insertQuery = @"
                //INSERT INTO UserTable (UserName, UserSex, UserBirthDay, UserMobilePhone)
                //VALUES (@UserName, @UserSex, @UserBirthDay, @UserMobilePhone)";
                //        using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                //        {
                //            cmd.Parameters.AddWithValue("@UserName", _userTable.UserName);
                //            cmd.Parameters.AddWithValue("@UserSex", _userTable.UserSex);
                //            cmd.Parameters.AddWithValue("@UserBirthDay", _userTable.UserBirthDay);
                //            cmd.Parameters.AddWithValue("@UserMobilePhone", _userTable.UserMobilePhone);
                //            cmd.ExecuteNonQuery();
                //        }
                //    }
                //// 第二種方法（作法類似後續的 Edit / Delete動作）
                //// 資料來源  https://msdn.microsoft.com/en-us/library/jj592676(v=vs.113).aspx
                ////              https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.entityframeworkcore.entitystate
                //_db.Entry(_userTable).State = Microsoft.EntityFrameworkCore.EntityState.Added;  // 確認新增一筆（狀態：Added）
                //_db.SaveChanges();
                return Content(" 新增一筆記錄，成功！"); // 新增成功後，出現訊息（字串）。
                //return RedirectToAction("List");
            }
            else
            {
                // 搭配 ModelState.IsValid，如果驗證沒過，就出現錯誤訊息。
                ModelState.AddModelError("Value1", " 自訂錯誤訊息(1) "); // 第一個輸入值是 key，第二個是錯誤訊息（字串）
                ModelState.AddModelError("Value2", " 自訂錯誤訊息(2) ");
                return View(); // 將錯誤訊息，返回並呈現在「新增」的檢視畫面上
            }
        }

        //========================= 新增、刪除、修改。三者都會用到！！
        // Entity states and .SaveChanges()方法
        // 資料來源 https://docs.microsoft.com/zh-tw/ef/ef6/saving/change-tracking/entity-state
        //=========================
        /* An entity can be in one of five states as defined by the EntityState enumeration. These states are:
        ** Added: the entity is being tracked by the context but does not yet exist in the database
        已新增：實體正在由內容 (context) 追蹤，但尚未存在於資料庫中
        ** Unchanged: the entity is being tracked by the context and exists in the database, and its property values have not changed from the values in the database
        未變更：實體正在由內容 (context) 追蹤，而且存在於資料庫中，且其屬性值未與資料庫中的值變更
        ** Modified: the entity is being tracked by the context and exists in the database, and some or all of its property values have been modified
        已修改：實體正在由內容 (context) 追蹤，而且存在於資料庫中，且部分或全部的屬性值已修改
        ** Deleted: the entity is being tracked by the context and exists in the database, but has been marked for deletion from the database the next time SaveChanges is called
        已刪除：實體正在由內容 (context) 追蹤，而且存在於資料庫中，但在下一次呼叫 .SaveChanges()方法 時已標示為從資料庫刪除
        ** Detached: the entity is not being tracked by the context
        已卸離：實體未被內容 (context) 追蹤
        //---------------------------------------------------------------------
        SaveChanges does different things for entities in different states:
        ** Unchanged entities are not touched by SaveChanges. Updates are not sent to the database for entities in the Unchanged state.
        .SaveChanges()方法 不會觸及未變更的實體。 針對處於未變更狀態的實體，不會將更新傳送至資料庫。
        ** Added entities are inserted into the database and then become Unchanged when SaveChanges returns.
        新增的實體會插入資料庫，然後在 .SaveChanges()方法 傳回時變成未變更。
        ** Modified entities are updated in the database and then become Unchanged when SaveChanges returns.
        修改過的實體會在資料庫中更新，然後在 .SaveChanges()方法傳回時變成未變更。
        ** Deleted entities are deleted from the database and are then detached from the context.
        刪除的實體會從資料庫中刪除，然後從內容卸離。
        相關範例請看 https://docs.microsoft.com/zh-tw/ef/ef6/saving/change-tracking/entity-state
     */
        //=========================
        // EntityState 列舉    https://docs.microsoft.com/zh-tw/dotnet/api/system.data.entitystate?view=netframework-4.8
        //=========================
        /*   命名空間:   System.Data  (支援 .NET Framewrok 4.x版)
        組件:   System.Data.Entity.dll
        Added	4
        物件是新的、已經加入至物件 內容 (context) ，而且尚未呼叫 SaveChanges() 方法。 儲存變更之後，物件狀態會變更為 Unchanged。
        處於 Added 狀態的物件在 ObjectStateEntry 中沒有原始值。
        Deleted	8
        已經從物件內容 (context) 中刪除物件。 儲存變更之後，物件狀態會變更為 Detached。
        Detached	1
        此物件存在，但是沒有追蹤此物件。 在已經建立實體之後而在實體加入至物件內容 (context) 之前，實體就會處於這種狀態中。
        在已經透過呼叫 Detach(Object) 方法從內容中移除實體後，或是使用 NoTrackingMergeOption 載入實體的話，實體也會處於這種狀態中。 沒有任何 ObjectStateEntry 執行個體會與處於 Detached 狀態的物件相關聯。
        Modified	16
        物件上的其中一個純量屬性已修改，而且尚未呼叫 .SaveChanges() 方法。
        已呼叫 Modified 方法時，在沒有變更追蹤 Proxy 的 POCO 實體中，已修改之屬性的狀態會變更為 DetectChanges()。 儲存變更之後，物件狀態會變更為 Unchanged。
        Unchanged	2
        自從此物件附加至內容 (context) ，或者自從上一次呼叫 .SaveChanges() 方法以來，此物件就沒有修改過。
        物件內容 (context) 必須了解物件狀態，才能將變更儲存回資料來源。 ObjectStateEntry 物件會儲存 EntityState 資訊。
        SaveChanges 的 ObjectContext 方法會處理附加至內容 (context) 的實體，並根據每個物件的 EntityState 來更新資料來源。 如需詳細資訊，請參閱 建立、加入、修改和刪除物件。
        物件內部的物件狀態是由 ObjectStateManager 所管理的。 若要找出物件狀態，請呼叫下列其中一個 ObjectStateManager 方法：TryGetObjectStateEntry、GetObjectStateEntry 或 GetObjectStateEntries。 State 的 ObjectStateEntry 屬性會定義物件的狀態。
        */
        //===================================
        //== 列表（Master） ==  暫無分頁功能。      分頁，在下方的 IndexPage動作
        //      EF Core v8.0版，如果資料表裡面有NULL（空值）則會報錯。但舊版 EF Core v7.0.9仍可正常運作。
        //===================================
        public async Task<IActionResult> List()
        {
            ////第一種寫法：  //*** 查詢結果是一個 IQueryable **************************
            //IQueryable<UserTable> ListAll = from _userTable in _db.UserTables
            //                                                      select _userTable;
            ////// 或是寫成
            ////var ListAll = from m in _db.UserTables
            ////              select m;
            ////*** 使用 IQueryable的好處是什麼？？************************************
            ///* The method uses "LINQ to Entities" to specify the column to sort by.The code creates an IQueryable variable
            //     before the switch statement, modifies it in the switch statement, and calls the ".ToList()" method after the
            //     switch statement.When you create and modify IQueryable variables, no query is sent to the database.
            //     The query is not executed until you convert the IQueryable object into a collection by calling a method such as ".ToList()".
            //     （直到程式的最後，你把查詢結果 IQueryable，呼叫.ToList()時，這段LINQ才會真正被執行！）
            //     Therefore, this code results in a single query that is not executed until the return View statement.
            // (1) http://blog.darkthread.net/post-2012-10-23-iqueryable-experiment.aspx
            //      ......發現 IQueryable<T> 是在 Server 端作過濾, 再將結果傳回 Client 端, 故若為「資料庫」存取, 應採用 IQueryable<T>
            // (2) http://jasper-it.blogspot.tw/2015/01/c-ienumerable-ienumerator.html
            //      ......在「資料庫」相關的環境下, 用 IQueryable<T> 的效能會比 IEnumerable<T> 來得好.  */
            ////*****************************************************************************
            ////*** 查詢結果 ListAll 是一個 IQueryable ***
            ////if (ListAll == null) // 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）。所以這段if辨別不了」
            //if (ListAll.Any() == false)  // 可運作
            ////if (ListAll.GetEnumerator().MoveNext() == false)   // 可運作
            //{   // 找不到這一筆記錄     //return HttpNotFound();   // .NET Core無此寫法。
            //    return NotFound();
            //    //return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            //}
            //else
            //{
            //    return View(await ListAll.ToListAsync());  // 執行 .ToList()方法後才真正運作，產生查詢的「結果(result)」。
            //    // 最後的 .ToList()方法，就算不寫也能運作，為什麼呢？ ...... 請看下面 Q&A的說明。
            //}
            ////第二種寫法：  // https://docs.microsoft.com/zh-tw/aspnet/mvc/overview/getting-started/introduction/accessing-your-models-data-from-a-controller
            var ListAll = _db.UserTables;
            if (ListAll == null) //簡寫。簡單的寫法
            {
                // 找不到任何記錄。也可寫成 return NotFound();
                return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            }
            else
            {
                return View(await ListAll.ToListAsync()); //直接把 UserTables的全部內容 列出來
                //翻譯成SQL指令的成果，跟第一種方法相同。
            }
            //// ADO.NET的寫法
            //List<UserTable> userList = new List<UserTable>();
            //string connectionString = _connectionString; // 請替換為您的 MySQL 連接字串
            //using (MySqlConnection conn = new MySqlConnection(connectionString))
            //{
            //    await conn.OpenAsync();
            //    string selectQuery = "SELECT UserId, UserName, UserSex, UserBirthDay, UserMobilePhone FROM UserTable";
            //    using MySqlCommand cmd = new MySqlCommand(selectQuery, conn);
            //    using MySqlDataReader reader = cmd.ExecuteReader();
            //    if (!reader.HasRows)
            //    {
            //        return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            //    }
            //    while (await reader.ReadAsync())
            //    {
            //        UserTable user = new UserTable
            //        {
            //            UserId = reader.GetInt32("UserId"),
            //            UserName = reader.GetString("UserName"),
            //            UserSex = reader.GetString("UserSex"),
            //            UserBirthDay = reader.IsDBNull(reader.GetOrdinal("UserBirthDay")) ? (DateTime?)null : reader.GetDateTime("UserBirthDay"),
            //            UserMobilePhone = reader.GetString("UserMobilePhone")
            //        };
            //        userList.Add(user);
            //    }
            //}
            //return View(userList);
            ////簡寫 - 第二種寫法的精S簡版。簡單的寫法
            //if (_db.UserTables == null)
            //{   // 找不到任何記錄。也可寫成 return NotFound();
            //    return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            //}
            //else
            //{
            //    return View(await _db.UserTables.ToListAsync());   //直接把 UserTables的全部內容 列出來
            //    翻譯成SQL指令的成果，跟第一種方法相同。
            //}
            //*** Q & A *****************
            /*  有人問到「上面的程式碼，最後 return View()的時候，不加上 .ToList()方法也能運作」這是為什麼？
            //****************************
              A :  微軟官方文件 - 將"查詢"的結果 儲存在"記憶體"中
                    https://docs.microsoft.com/zh-tw/dotnet/csharp/linq/store-the-results-of-a-query-in-memory
             查詢，基本上是如何擷取和組織資料的一組指令。 要求結果中的每個後續項目時，會延遲 (Lazily) 執行查詢。
             (1)  當您使用 foreach迴圈逐一查看結果時，會將項目傳回為 "已存取"。
                 （註解：回答上面問題。因為檢視畫面 View裡有執行 foreach迴圈，所以上面程式不加上 .ToList()方法也會跑。）
             (2)  若要查詢並儲存其結果，卻「不」用執行 foreach 迴圈，只要在查詢變數上，呼叫下面其中一種方法即可做到︰
                   .ToList     .ToArray   .ToDictionary   .ToLookup
            --------------------------------------------------------------------------------------------
               結  論 --
                  如果 (2)這些方法將查詢 "具體化"到 List<T> 或 Array 類型，則會視為傳回「查詢結果(result)」，而不是查詢本身。
                  如果您採用 (1)的方法，不加上 .ToList()，而直接由 foreach迴圈來做。在這種情況下，它是傳回的「查詢」本身，而不是查詢結果 (result)。
                  請看 https://docs.microsoft.com/zh-tw/dotnet/csharp/linq/return-a-query-from-a-method
               補充 -- .AsEnumerable() 和 .ToList() 兩者的差異？
               請看  https://www.cnblogs.com/mainz/archive/2011/04/08/2009485.html  （簡體中文） */
        }

        //===================================
        //== 列出一筆記錄的明細（Details） ==
        //===================================
        //[HttpPost]    // 改成[HttpPost] 會報錯。請輸入網址，看見了什麼？？？？ /UserDB/Details?_ID=4
        ////                 // 錯誤訊息 -- '/' 應用程式中發生伺服器錯誤。        找不到資源。
        //   可以對照 最底下三個 Search動作，可以更清楚得知這個錯誤與修正方法。
        [HttpGet]
        public IActionResult Details(int? _ID = 1) // 網址 http://xxxxxx/UserDB/Details?_ID=1
        {
            // 如果沒有修改路由  /Program.cs，網址輸入 http://xxxxxx/UserDB/Details/2 會報錯！
            if (_ID == null || _ID.HasValue == false)
            {
                //// 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content("沒有輸入文章編號（_ID）");
            }

            //// 第一種寫法：========================================
            //IQueryable<UserTable> ListOne = from _userTable in _db.UserTables
            //              where _userTable.UserId == _ID    // 比上個範例（List範本）多出這一句話
            //              select _userTable;
            ////也可以寫成下面這樣：
            ////var ListOne = from m in _db.UserTables
            ////              where m.UserId == _ID
            ////              select m;
            ////*** 查詢結果 ListAll 是一個 IQueryable ***
            ////if (ListOne == null) // 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）。所以這段if辨別不了」
            //if (ListOne.Any() == false)  // 可運作
            ////if (ListOne.GetEnumerator().MoveNext() == false)   // 可運作
            //{    // 找不到這一筆記錄
            //     //    return NotFound();
            //    return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            //}
            //else
            //{
            //    return View(ListOne.FirstOrDefault());
            //}
            //// 第二種寫法： 透過 .Where() =============================
            IQueryable<UserTable> ListOne2 = _db.UserTables.Where(x => x.UserId == _ID);
            //*** 查詢結果 ListAll 是一個 IQueryable ***
            //if (ListOne2 == null) // 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）。所以這段if辨別不了」
            if (ListOne2.Any() == false) // 可運作
                //if (ListOne2.GetEnumerator().MoveNext() == false)   // 可運作
            {
                // 找不到這一筆記錄
                //    return NotFound();
                return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            }
            else
            {
                return View(ListOne2.FirstOrDefault()); // 翻譯成SQL指令的結果，同上（第一種方法）。
            }
            //// ADO .NET的寫法
            //UserTable user = null;
            //string connectionString = _connectionString; // 請替換為您的 MySQL 連接字串
            //using (MySqlConnection conn = new MySqlConnection(connectionString))
            //{
            //    conn.Open();
            //    string selectQuery = "SELECT UserId, UserName, UserSex, UserBirthDay, UserMobilePhone FROM UserTable WHERE UserId = @UserId";
            //    using MySqlCommand cmd = new MySqlCommand(selectQuery, conn);
            //    cmd.Parameters.AddWithValue("@UserId", _ID);
            //    using MySqlDataReader reader = cmd.ExecuteReader();
            //    if ( reader.Read())
            //    {
            //        user = new UserTable
            //        {
            //            UserId = reader.GetInt32("UserId"),
            //            UserName = reader.GetString("UserName"),
            //            UserSex = reader.GetString("UserSex"),
            //            UserBirthDay = reader.IsDBNull(reader.GetOrdinal("UserBirthDay")) ? (DateTime?)null : reader.GetDateTime("UserBirthDay"),
            //            UserMobilePhone = reader.GetString("UserMobilePhone")
            //        };
            //    }
            //}
            //if (user == null)
            //{
            //    return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            //}
            //else
            //{
            //    return View(user);
            //}
            //// 第三種寫法： 透過 .FirstOrDefault() =========================
            //var ListOne3 = _db.UserTables.FirstOrDefault(b => b.UserId == _ID);     // 寫法3-1
            //var ListOne3 = _db.UserTables.First(b => b.UserId == _ID);               // 寫法3-2
            //// 使用Enumerable.First()找不到資料的時候會出錯，而使用Enumerable.FirstOrDefault()找不到資料會給予預設值
            //// https://shunnien.github.io/2015/11/04/first-with-the-firstordefault/
            //// https://learn.microsoft.com/zh-tw/dotnet/api/system.linq.enumerable.firstordefault?view=net-7.0
            //var ListOne3 = _db.UserTables.SingleOrDefault(b => b.UserId == _ID);    // 寫法3-3
            //// .Single()傳回序列中符合指定之條件的唯一一個元素，如果有"一個以上"這類元素，則擲回例外狀況。
            //// .First()可能找到多個元素筆記錄，但只傳回指定之序列中的第一個項目。如果找不到（Null）就會報錯。
            //// https://learn.microsoft.com/zh-tw/dotnet/api/system.linq.enumerable.single?view=net-7.0
            //// 翻譯成SQL指令的結果，同上（第一種方法）。
            //if (ListOne3 == null)  // 可運作
            //{    // 找不到這一筆記錄
            //    return NotFound();
            //}
            //else
            //{
            //    return View(ListOne3);
            //}
            //// 第四種寫法：透過 .Find()
            //UserTable ListOne4 = _db.UserTables.Find(_ID);    // 翻譯成SQL指令的結果，同上（第一種方法）
            //if (ListOne4 == null)  // 可運作
            //{   // 找不到這一筆記錄
            //    return NotFound();
            //}
            //else
            //{
            //    return View(ListOne4);
            //}
            //// 第五種寫法 Only for EF Core：傳統T-SQL指令（透過 .FromSqlRaw() ）但不需要學ADO.NET
            //// https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/advanced
            //SqlParameter _IDparameter = new SqlParameter("IDparameter", _ID);
            //// 避免資料隱碼、注入攻擊（SQL Injection）。需搭配 Microsoft.Data.SqlClient命名空間 (請自己手動安裝NuGet )。
            //var ListOne5 = _db.UserTables
            //                      .FromSqlRaw("SELECT * FROM UserTable with(nolock) WHERE UserID = @IDparameter", _IDparameter)
            //                      // 第一個參數，SQL指令。  第二個參數，＠參數
            //                      // EF Core 3.0以前使用的是  .FromSql()方法  https://docs.microsoft.com/zh-tw/ef/core/querying/raw-sql
            //                      .OrderBy(d => d.UserId)  // 如果不使用 OrderBy就直接搭配 FirstOrDefault（只取出一筆記錄）可能會有隱藏的問題。
            //                                               // 這是微軟的建議。warn: Microsoft.EntityFrameworkCore.Query[10103]
            //                                               //           The query uses the 'First' / 'FirstOrDefault' operator without 'OrderBy' and filter operators.This may lead to unpredictable results.
            //                      .FirstOrDefault();
            //if (ListOne5 == null)  // 可運作
            //{   // 找不到這一筆記錄
            //    return NotFound();
            //}
            //else
            //{
            //    return View(ListOne5);
            //}
        }

        //===================================
        //== 刪除 ==  （不搭配路由 (Route) 就無法運作。必須自己手動加入 HTML Hidden欄位）
        //===================================
        //== 刪除前的 Double-Check，先讓您確認這筆記錄的內容？
        //[HttpGet]  加上這一句也無法解決「找不到 _ID」的問題
        public IActionResult Delete(int? _ID) // 網址 http://xxxxxx/UserDB/Delete?_ID=1
        {
            if (_ID == null)
            {
                // 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content(" 沒有輸入文章編號（_ID）");
            }

            // 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
            UserTable? ut = _db.UserTables.Find(_ID);
            if (ut == null)
            {
                // 找不到這一筆記錄
                // return NotFound();
                return Content(" 找不到這一筆記錄！");
            }
            else
            {
                return View(ut);
            }
        }

        //== 真正刪除這一筆，並回寫資料庫 ===============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // 避免CSRF攻擊  https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application#overpost
        // 避免CSRF攻擊。在FormTagHelper - 如果有寫 action屬性、或是Method = Get就會啟動此功能。設定為true。
        // https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.taghelpers.formtaghelper
        // 避免 "刪除" 一筆記錄的安全漏洞 http://stephenwalther.com/archive/2009/01/21/asp-net-mvc-tip-46-ndash-donrsquot-use-delete-links-because
        //           如果您希望將刪除的動作，合併在一起，一次解決，請看下面文章的最後一個範例（P.S. 可能有安全漏洞）。
        //           https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/examining-the-details-and-delete-methods
        public IActionResult DeleteConfirm(int _ID)
        {
            //return Content(_ID.ToString());  //為什麼無法刪除？找不到這一筆記錄？是不是沒有傳遞_ID過來這裡呢？
            if (ModelState.IsValid) // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {
                //// 第二種方法（作法類似後續的 Edit動作）
                UserTable? ut = _db.UserTables.Find(_ID);
                if (ut == null)
                {
                    // 找不到這一筆記錄
                    return Content(" 刪除時，找不到這一筆記錄！");
                }
                else
                {
                    _db.Entry(ut).State = Microsoft.EntityFrameworkCore.EntityState.Deleted; // 確認刪除一筆（狀態：Deleteed）
                    _db.SaveChanges();
                    //**** 刪除以後，必須執行 .SaveChanges()方法，才能真正去DB刪除這一筆記錄 ****
                }

                //// ADO .NET 的寫法
                //string connectionString = _connectionString; // 請替換為您的 MySQL 連接字串
                //using (MySqlConnection conn = new MySqlConnection(connectionString))
                //{
                //    conn.Open();
                //    string selectQuery = "SELECT * FROM UserTable WHERE UserId = @UserId";
                //    using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                //    {
                //        cmd.Parameters.AddWithValue("@UserId", _ID);
                //        using (MySqlDataReader reader = cmd.ExecuteReader())
                //        {
                //            if (!reader.HasRows)
                //            {
                //                return Content(" 刪除時，找不到這一筆記錄！");
                //            }
                //        }
                //    }
                //    string deleteQuery = "DELETE FROM UserTable WHERE UserId = @UserId";
                //    using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                //    {
                //        cmd.Parameters.AddWithValue("@UserId", _ID);
                //        cmd.ExecuteNonQuery();
                //    }
                //}
                //return Content("記錄已成功刪除！");
                //// 第三種方法。必須先找到這一筆記錄。找得到，才能刪除！
                //UserTable ut = _db.UserTables.Find(_ID);   // https://www.youtube.com/watch?v=cINtgwbG8zo
                //if (ut == null)
                //{   // 找不到這一筆記錄
                //    return Content(" 刪除時，找不到這一筆記錄！");
                //}
                //else
                //{
                //    _db.UserTables.Remove(ut);
                //    _db.SaveChanges();  //**** 刪除以後，必須執行 .SaveChanges()方法，才能真正去DB刪除這一筆記錄 ****
                //}
                //return Content(" 刪除一筆記錄，成功！");    // 刪除成功後，出現訊息（字串）。
                //return RedirectToAction("List");
                return RedirectToAction(nameof(List)); // 提升程式的維護性，常用在"字串"上。
            }
            else
            {
                // 搭配 ModelState.IsValid，如果驗證沒過，就出現錯誤訊息。
                ModelState.AddModelError("Value1", " 自訂錯誤訊息(1) ");
                ModelState.AddModelError("Value2", " 自訂錯誤訊息(2) ");
                return View(); // 將錯誤訊息，返回並呈現在「刪除」的檢視畫面上
            }
        }

        //=================================================
        //==  舊版的檢視畫面，HTML Helper （可正常運作 / 不用搭配路由）=====
        //=================================================
        public IActionResult Delete2017(int? _ID) // 網址 http://xxxxxx/UserDB/Delete?_ID=1
        {
            if (_ID == null)
            {
                // 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content(" 沒有輸入文章編號（_ID）");
            }

            // 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
            UserTable? ut = _db.UserTables.Find(_ID);
            if (ut == null)
            {
                // 找不到這一筆記錄
                // return NotFound();
                return Content(" 找不到這一筆記錄！");
            }
            else
            {
                return View(ut);
            }
        }

        //== 真正刪除這一筆，並回寫資料庫 ===============
        [HttpPost, ActionName("Delete2017")]
        [ValidateAntiForgeryToken] // 避免CSRF攻擊  https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application#overpost
        public IActionResult Delete2017Confirm(int _ID)
        {
            //return Content(_ID.ToString());  //為什麼無法刪除？找不到這一筆記錄？是不是沒有傳遞_ID過來這裡呢？
            if (ModelState.IsValid) // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {
                //// 第二種方法（作法類似後續的 Edit動作）
                UserTable? ut = _db.UserTables.Find(_ID);
                if (ut == null)
                {
                    // 找不到這一筆記錄
                    return Content(" 刪除時，找不到這一筆記錄！");
                }
                else
                {
                    _db.Entry(ut).State = Microsoft.EntityFrameworkCore.EntityState.Deleted; // 確認刪除一筆（狀態：Deleteed）
                    _db.SaveChanges();
                    //**** 刪除以後，必須執行 .SaveChanges()方法，才能真正去DB刪除這一筆記錄 ****
                }

                return Content(" 刪除一筆記錄，成功！"); // 刪除成功後，出現訊息（字串）。
                //return RedirectToAction("List");
            }
            else
            {
                // 搭配 ModelState.IsValid，如果驗證沒過，就出現錯誤訊息。
                ModelState.AddModelError("Value1", " 自訂錯誤訊息(1) ");
                ModelState.AddModelError("Value2", " 自訂錯誤訊息(2) ");
                return View(); // 將錯誤訊息，返回並呈現在「刪除」的檢視畫面上
            }
        }

        //===================================
        //== 修改（編輯）畫面 #1 ==       輸入 UserTable類別
        //===================================
        [HttpGet]
        public ActionResult Edit(int? _ID = 1)
        {
            if (_ID == null)
            {
                // 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content("請輸入 _ID 編號");
            }

            // 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
            UserTable? ut = _db.UserTables.Find(_ID);
            if (ut == null)
            {
                // 找不到這一筆記錄
                // return NotFound();
                return Content("找不到任何記錄");
            }
            else
            {
                return View(ut);
            }
        }

        //== 修改（更新），回寫資料庫 #1 ============ 注意！這裡的輸入值是一個 UserTable
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken] // 避免CSRF攻擊  https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application#overpost
        // [Bind(...)] 也可以寫在 Model的類別檔裡面，就不用重複地寫在新增、刪除、修改每個動作之中。
        // 可以避免 overposting attacks （過多發佈）攻擊  http://www.cnblogs.com/Erik_Xu/p/5497501.html
        // 參考資料 http://blog.kkbruce.net/2011/10/aspnet-mvc-model-binding6.html
        public ActionResult EditConfirm(
            [Bind("UserId, UserName, UserSex, UserBirthDay, UserMobilePhone")]
            UserTable _userTable)
        {
            // https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud  關於大量指派（overposting / 過多發佈）的安全性注意事項
            if (_userTable == null)
            {
                // 沒有輸入內容，就會報錯 - Bad Request
                return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid) // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {
                // 參考資料： （理論）http://www.entityframeworktutorial.net/basics/entity-states.aspx
                // （有CRUD完整範例 for .NET Core） https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud
                //// 第一種寫法： *** 注意！.NET Core的 命名空間 改變了 ***
                ///此方法可用來一次新增/刪除資料表內容
                //_db.Entry(_userTable).State = Microsoft.EntityFrameworkCore.EntityState.Modified;  // 確認被修改（狀態：Modified）
                //_db.SaveChanges();
                //// 第二種寫法：========================================= (start)

                #region

                //// 此方法可用來更新資料表部分內容(EX:會員登入後修改個人資料)
                //// 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
                UserTable? ut = _db.UserTables.Find(_userTable.UserId);
                if (ut == null)
                {
                    // 找不到這一筆記錄
                    return NotFound();
                }
                else
                {
                    ut.UserName = _userTable.UserName; // 修改後的值
                    ut.UserSex = _userTable.UserSex;
                    ut.UserBirthDay = _userTable.UserBirthDay;
                    ut.UserMobilePhone = _userTable.UserMobilePhone;
                    _db.SaveChanges(); // 回寫資料庫（進行修改）
                }

                //    // ADO .NET 的寫法
                //    string connectionString = _connectionString; // 請替換為您的 MySQL 連接字串
                //    using (MySqlConnection conn = new MySqlConnection(connectionString))
                //    {
                //        conn.Open();
                //        // 檢查記錄是否存在
                //        string selectQuery = "SELECT * FROM UserTable WHERE UserId = @UserId";
                //        using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn))
                //        {
                //            selectCmd.Parameters.AddWithValue("@UserId", _userTable.UserId);
                //            using MySqlDataReader reader = selectCmd.ExecuteReader();
                //            if (!reader.HasRows)
                //            {
                //                return NotFound();
                //            }
                //        }
                //        // 更新記錄
                //        string updateQuery = @"
                //UPDATE UserTable
                //SET UserName = @UserName,
                //    UserSex = @UserSex,
                //    UserBirthDay = @UserBirthDay,
                //    UserMobilePhone = @UserMobilePhone
                //WHERE UserId = @UserId";
                //        using MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                //        updateCmd.Parameters.AddWithValue("@UserName", _userTable.UserName);
                //        updateCmd.Parameters.AddWithValue("@UserSex", _userTable.UserSex);
                //        updateCmd.Parameters.AddWithValue("@UserBirthDay", _userTable.UserBirthDay);
                //        updateCmd.Parameters.AddWithValue("@UserMobilePhone", _userTable.UserMobilePhone);
                //        updateCmd.Parameters.AddWithValue("@UserId", _userTable.UserId);
                //        updateCmd.ExecuteNonQuery();
                //    }
                //// 第二種寫法：========================================= (end)

                #endregion

                //// 第三種寫法：========================================= (start)
                //// 當網頁 UI 包含了 "所有"實體中的欄位，並且可以更新其中的任何一個欄位時，您可以使用此方法。
                ////  (請 "不要" 使用此程式碼更新您的專案。此程式碼僅作為展示 / 某一可供選擇的方法。)

                #region

                //// https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud  HttpPost Edit 程式碼的替代方案：建立及連結
                //_db.UserTables.Update(_userTable);
                //_db.SaveChanges();
                //// 第三種寫法：========================================= (end)

                #endregion

                //// 第四種寫法：TryUpdateModel ============================== (start)
                //// 跟以前MVC 5的寫法雷同(不建議學習)。
                //// https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud?view=aspnetcore-5.0#update-the-edit-page
                //// 第四種寫法：TryUpdateModel ============================== (end)
                //return Content(" 更新一筆記錄，成功！");    // 更新成功後，出現訊息（字串）。
                //return RedirectToAction("List");
                return RedirectToAction(nameof(List)); // 提升程式的維護性，常用在"字串"上。
            }
            else
            {
                //return View(_userTable);  // 若沒有修改成功，則列出原本畫面
                return Content(" *** 更新失敗！！*** ");
            }
        }

        //===================================
        //== 修改（編輯）畫面 #2 ==      [與上一個範例的差異] 輸入 _ID （文章編號）而非 UserTable類別
        //===================================
        public ActionResult Edit2(int? _ID) // 跟 Edit動作 #1相同，沒有變化
        {
            if (_ID == null)
            {
                // 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content("請輸入 _ID 編號");
            }

            // 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
            UserTable? ut = _db.UserTables.Find(_ID);
            if (ut == null)
            {
                // 找不到任何記錄。 .NET Core改用 NotFound();
                // return NotFound();
                return Content("找不到任何記錄");
            }
            else
            {
                return View(ut);
            }
        }

        //== 修改（更新），回寫資料庫 #2 ============  [與上一個範例的差異] 輸入 _ID （文章編號）而非 UserTable類別
        [HttpPost, ActionName("Edit2")]
        [ValidateAntiForgeryToken] // 避免XSS、CSRF攻擊  https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application#overpost
        // 重點！！與上一個範例不同的地方    //***** 輸入 _ID而非 UserTable                                       //***** 輸入 _ID而非 UserTable
        public async Task<IActionResult> Edit2Confirm(
            [Bind("UserId, UserName, UserSex, UserBirthDay, UserMobilePhone")]
            int? _ID)
        {
            // 這個寫法必須使用「非同步、異步 (Async)」才行。
            if (String.IsNullOrEmpty(_ID.ToString()))
            {
                // 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content("請輸入 _ID 編號");
            }

            if (ModelState.IsValid) // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {
                //// ==================================================
                //// =========    不建議學這種"簡易的"作法，有安全上的疑慮！！  =======
                //// ==================================================

                #region

                //// 如果寫成 TryUpdateModel(_userTable) 則報錯！無法運作。寫成 TryUpdateModel(_userTable.UserId) 成功。
                //if (ModelState.IsValid && await TryUpdateModelAsync(_db.UserTables.Find(_ID)))
                //{
                //    await _db.SaveChangesAsync();
                //    return RedirectToAction("List");
                //    //return Content(" 更新一筆記錄，成功！");    // 更新成功後，出現訊息（字串）。
                //}

                #endregion

                //// ==================================================
                //// 如果寫成 TryUpdateModel(_userTable) 無法運作。寫成 TryUpdateModel(_userTable.UserId) 則成功。
                //// https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.tryupdatemodelasync
                //// TryUpdateModel 會透過你從表單回傳的資料，只要「名稱」一樣就會自己繫結（綁定 / Binding）。這是Model Binding。
                //// ==================================================
                if (await TryUpdateModelAsync<UserTable>(_db.UserTables.Find(_ID), "",
                        s => s.UserName, s => s.UserSex, s => s.UserBirthDay, s => s.UserMobilePhone))
                {
                    await _db.SaveChangesAsync();
                    //return Content(" 更新一筆記錄，成功！");    // 更新成功後，出現訊息（字串）。
                    return RedirectToAction("List"); // 修改成功，回到首頁（所有資料的列表）
                }

                return Content(" *** 更新失敗！！*** ");
            }
            else
            {
                return Content(" *** ModelState.IsValid 驗證失敗！！*** ");
            }
        }

        //===================================
        //== 分頁#1 ==  基本觀念說明。  LINQ的 .Skip() 與 .Take()
        // https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-partitioning
        //===================================
        public ActionResult IndexPage(int _ID = 1) // _ID變數，目前位於第幾頁（目前頁數）？為了避免有人不輸入，所以預設為1
        {
            // PageSize變數，每一頁，要展示幾筆記錄？(可記錄於設定檔中 EX: appsettings.json)
            int PageSize = 3;
            // NowPageCount，目前正在觀賞這一頁的紀錄。目前正在第幾頁？
            int NowPageCount = 0;
            if (_ID > 0 || String.IsNullOrEmpty(_ID.ToString()))
            {
                NowPageCount = (int)((_ID - 1) * PageSize); // PageSize，每頁展示3筆紀錄（上面設定過了）
            }

            // 這段指令的 .Skip()與 . Take()，其實跟T-SQL指令的 offset...fetch next....很類似（SQL 2012起可用）
            var ListAll = (from _userTable in _db.UserTables
                orderby _userTable.UserId // 若寫 descending ，則是反排序（由大到小）
                select _userTable).Skip(NowPageCount).Take(PageSize);
            // .Skip() 從哪裡開始（忽略前面幾筆記錄）。 .Take()呈現幾筆記錄
            //    // ADO .NET 的寫法
            //    var ListAll = new List<UserTable>();
            //    string connectionString = "YourMySQLConnectionStringHere"; // 請替換為實際的 MySQL 連接字串
            ////計算符合條件的記錄總數
            //int recordCount = 0;
            //string countQuery = "SELECT COUNT(*) FROM UserTables";
            //using (MySqlConnection countConnection = new MySqlConnection(connectionString))
            //{
            //    MySqlCommand countCommand = new MySqlCommand(countQuery, countConnection);
            //    countConnection.Open();
            //    recordCount = Convert.ToInt32(countCommand.ExecuteScalar());
            //}
            //// 查詢符合條件的記錄
            //    string query = @"
            //SELECT UserId, UserName, UserSex, UserBirthDay, UserMobilePhone
            //FROM UserTables
            //ORDER BY UserId
            //LIMIT @PageSize OFFSET @NowPageCount"; //跳掉NowPageCount的紀錄數量,只取PageSize筆數的資料
            //    using (MySqlConnection connection = new MySqlConnection(connectionString))
            //    {
            //        MySqlCommand command = new MySqlCommand(query, connection);
            //        command.Parameters.AddWithValue("@NowPageCount", NowPageCount);
            //        command.Parameters.AddWithValue("@PageSize", PageSize);
            //        connection.Open();
            //        using MySqlDataReader reader = command.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            var user = new UserTable
            //            {
            //                UserId = reader.GetInt32(0),
            //                UserName = reader.IsDBNull(1) ? null : reader.GetString(1),
            //                UserSex = reader.IsDBNull(2) ? null : reader.GetString(2),
            //                UserBirthDay = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
            //                UserMobilePhone = reader.IsDBNull(4) ? null : reader.GetString(4)
            //            };
            //            ListAll.Add(user);
            //        }
            //    }
            //*** 查詢結果 ListAll 是一個 IQueryable ***
            //if (ListAll == null) // 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）。所以這段if辨別不了」
            if (ListAll.Any() == false) // 可運作
                //if (ListAll.GetEnumerator().MoveNext() == false)   // 可運作
            {
                // 找不到任何記錄。 .NET Core改用 NotFound();
                return Content(" 找不到「這一頁」的紀錄！"); // 或寫成 return NotFound();
            }
            else
            {
                return View(ListAll);
                //*** 查詢結果 ListAll 是一個 IQueryable
                //*** 使用 IQueryable的好處是什麼？？ ...... 完整說明，請看上面的 List()動作。
            }
        }

        //===================================
        //== 分頁#2 ==      手動加上「分頁列」
        //== 畫面下方，加入「上一頁」、「下一頁」、每十頁作間隔
        public ActionResult IndexPage2(int _ID = 1)
        {
            // _ID變數，目前位於第幾頁？
            // PageSize變數，每一頁，要展示幾筆記錄？
            int PageSize = 3;
            // RecordCount變數，符合條件的「總共」有幾筆記錄？
            int RecordCount = _db.UserTables.Count();
            // 如果您要搭配下一節的「搜尋」功能，請改成「搜尋關鍵字」的指令
            // NowPageCount，目前這一頁、您正在觀賞的這一頁。目前正在第幾頁？
            int NowPageCount = 0;
            if (_ID > 0 || String.IsNullOrEmpty(_ID.ToString()))
            {
                NowPageCount = (int)((_ID - 1) * PageSize); // PageSize，每頁展示3筆紀錄（上面已設定過）
            }

            // 這段指令的 .Skip()與 . Take()，其實跟T-SQL指令的 offset...fetch....很類似（MS SQL Server 2012起可用）
            var ListAll = (from _userTable in _db.UserTables
                orderby _userTable.UserId // 若寫 descending ，則是反排序（由大到小）
                select _userTable).Skip(NowPageCount).Take(PageSize); // .Skip() 從哪裡開始（忽略前面幾筆記錄）。 .Take()呈現幾筆記錄
            //*** 查詢結果 ListAll 是一個 IQueryable ***
            //if (ListAll == null) // 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）。所以這段if辨別不了」
            if (ListAll.Any() == false) // 可運作
                //if (ListAll.GetEnumerator().MoveNext() == false)   // 可運作
            {
                // 找不到任何記錄。 .NET Core改用 NotFound();
                return Content(" 找不到「這一頁」的任何記錄！"); // 或寫成 return NotFound();
            }
            else
            {
                //************** 比上一個範例  多的程式碼。 ******************************(start)
                // 畫面下方的「分頁列」。「每十頁」一間隔，分頁功能

                #region

                int Pages; // Pages變數，「總共需要幾頁」才能把所有紀錄展示完畢？
                if ((RecordCount % PageSize) > 0)
                {
                    //   %，除法，傳回餘數
                    Pages = ((RecordCount / PageSize) + 1); //  ( / )除法。傳回整數。
                    //  如果無法整除，有餘數，則需要多出一頁來呈現。
                }
                else
                {
                    Pages = (RecordCount / PageSize); //   ( / )除法。傳回整數
                }

                // StringBuilder 應用情境為：需要大量的字串處理時，例如字串的長短不一(時長時短),且數量龐大(5000,10k筆以上)。越複雜越大量的字串處理，StringBuilder的效能優勢越明顯。
                System.Text.StringBuilder sbPageList = new System.Text.StringBuilder();
                //===== 分頁功能（上一頁 / 下一頁）======== start ==

                #region

                if (Pages > 0)
                {
                    //有傳來「頁數(p)」，而且頁數正確（大於零），出現<上一頁>、<下一頁>這些功能
                    sbPageList.Append("<div align=\"center\">");
                    //** 可以把檔名刪除，只留下 ?_ID=  即可！一樣會運作，但IE 11會出現 JavaScript錯誤。**
                    //** 抓到目前網頁的「檔名」。 System.IO.Path.GetFileName(Request.PhysicalPath) **
                    if (_ID > 1)
                    {
                        sbPageList.Append("<a href=\"?_ID=" + (_ID - 1) + "\">[<<<上一頁]</a>");
                    }

                    sbPageList.Append(
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b><a href=\"http://127.0.0.1/\">[首頁]</a></b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    if (_ID < Pages)
                    {
                        sbPageList.Append("<a href=\"?_ID=" + (_ID + 1) + "\">[下一頁>>>]</a>");
                    }

                    #endregion

                    //===== 分頁功能（上一頁 / 下一頁）======== end ==
                    //===============================================
                    //====== MIS2000 Lab.自製的「每十頁」一間隔，分頁功能 === start ==

                    #region

                    sbPageList.Append("<hr width='97%' size=1>");
                    int block_page = 0;
                    block_page = _ID / 10; //--只取除法的整數成果（商），若有餘數也不去管它。
                    if (block_page > 0)
                    {
                        sbPageList.Append("<a href=\"_ID=" + (((block_page - 1) * 10) + 9) +
                                          "\"> [前十頁<<]  </a>&nbsp;&nbsp;");
                    }

                    for (int K = 0; K <= 10; K++)
                    {
                        if ((block_page * 10 + K) <= Pages)
                        {
                            //--- Pages 資料的總頁數。共需「幾頁」來呈現所有資料？
                            if (((block_page * 10) + K) == _ID)
                            {
                                //--- id 就是「目前在第幾頁」
                                sbPageList.Append("[<b>" + _ID + "</b>]" + "&nbsp;&nbsp;&nbsp;");
                            }
                            else
                            {
                                if (((block_page * 10) + K) != 0)
                                {
                                    sbPageList.Append("<a href=\"?_ID=" + (block_page * 10 + K) + "\">" +
                                                      (block_page * 10 + K) + "</a>");
                                    sbPageList.Append("&nbsp;&nbsp;&nbsp;");
                                }
                            }
                        }
                    } //for迴圈 end

                    if ((block_page < (Pages / 10)) & (Pages >= (((block_page + 1) * 10) + 1)))
                    {
                        sbPageList.Append("&nbsp;&nbsp;<a href=\"?id=" + ((block_page + 1) * 10 + 1) +
                                          "\">  [>>後十頁]  </a>");
                    }

                    sbPageList.Append("</div>");
                }

                #endregion

                //====== MIS2000 Lab.自製的「每十頁」一間隔，分頁功能 === end ==

                #endregion

                ViewBag.PageList = sbPageList.ToString();
                //************** 比上一個範例  多的程式碼。 ******************************(end)
                return View(ListAll);
            }
        }

        //===================================
        //== 分頁#3（微軟官方教材） ==  LINQ的 .Skip() 與 .Take()
        //      需搭配根目錄底下的PaginatedList.cs。由他來做分頁，查詢指令不可以寫分頁，全部列出即可。
        // https://learn.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-6.0
        //===================================
        public ActionResult IndexPage3(int? _ID = 1) // _ID變數，目前位於第幾頁？
        {
            // PageSize變數，每一頁，要展示幾筆記錄？
            int PageSize = 3;
            // NowPageCount，目前正在觀賞這一頁。目前正在第幾頁？
            int NowPageCount = 0;
            if (_ID > 0 || String.IsNullOrEmpty(_ID.ToString()))
            {
                NowPageCount = (int)((_ID - 1) * PageSize); // PageSize，每頁展示3筆紀錄（上面設定過了）
            }

            //************* 跟上一個範例不同之處 ***********************************************
            //************* 不能在查詢指令這裡做「分頁」，否則會出現異常！*****************
            // 需搭配根目錄底下的PaginatedList.cs。由他來做分頁，查詢指令不可分頁，乖乖地全部列出即可。
            var ListAll = (from _userTable in _db.UserTables
                orderby _userTable.UserId // 若寫 descending ，則是反排序（由大到小）
                select _userTable);
            //************* 跟上一個範例不同之處 ***********************************************
            //*** 查詢結果 ListAll 是一個 IQueryable ***
            //if (ListAll == null) // 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）。所以這段if辨別不了」
            if (ListAll.Any() == false) // 可運作
                //if (ListAll.GetEnumerator().MoveNext() == false)   // 可運作
            {
                // 找不到任何記錄。 .NET Core改用 NotFound();
                return Content(" 找不到這一筆記錄！"); // 或寫成 return NotFound();
            }
            else
            {
                // 需搭配根目錄底下的PaginatedList.cs。
                // .AsNoTracking() -- 抓最新的資料，避免被快取（緩存 Cached）
                // 將查詢結果（ListAll，多筆資料）轉換成「單一頁面」的內容。 該單一頁面會傳遞至檢視畫面。
                return View(PaginatedList<UserTable>.PagerCreate(ListAll.AsNoTracking(), _ID ?? 1, PageSize));
                // C# 8.0起的新功能。如果 Null 聯合運算子 ?? 不是 null，會傳回其左方運算元的值；否則它會評估右方運算元，並傳回其結果。
            }
        }

        //====================================
        //== 搜尋  關鍵字。類似上面的 列表（Master / List） ==
        //== https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-search
        //
        //== .Where() 與 .Contains()的寫法 https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-filtering
        //     (1) 搜尋  日期    .Where(o => o.OrderDate >= new DateTime(2003, 12, 1))
        //     (2) 搜尋  符合兩個條件（用&&）  .Where(o => o.OrderQty > orderQtyMin && order.OrderQty < orderQtyMax)
        //     (3) 搜尋  符合陣列裡面的「值」。產品模組ID符合  19/26/18。或是 產品尺寸符合 L/XL
        //                               .Where(p => (new int?[] { 19, 26, 18 }).Contains(p.ProductModelID) ||  (new string[] { "L", "XL" }).Contains(p.Size));
        //
        //== 中文範例（排序 / Sorting） https://www.cnblogs.com/slark/p/mvc5-ef6-bs3-get-started-pagedlist.html
        //===================================
        [HttpPost] // *** 重點設定 ***
        // 請透過「List範本」產生檢視畫面，來執行搜尋的成果。
        // 錯誤！ 第一個搜尋的動作，採用POST，所以URL輸入 http://xxxxxx/Day3/Search?_SearchWord=MVC  會報錯
        // 錯誤！ 直接在網址輸入 http://xxxxxx/Day3/Search/MVC  （需要修改 Program.cs下的  Route設定。把 id 改 _SearchWord 才行）
        // 自行輸入網址，需改成 [HttpGet]才行。請看下面的 Search3動作。
        [ValidateAntiForgeryToken] // 避免XSS、CSRF攻擊，需搭配 List檢視畫面下方的輸入表格
        public ActionResult Search(string _SearchWord)
        {
            // 首先，試試看，能否抓得到檢視頁面傳來的數值？
            // return Content("<h3> 檢視頁面傳來的 -- " + _SearchWord + "</h3>");
            ViewData["SW"] = _SearchWord;
            //===========================================
            //第一種寫法：Query Syntax，查詢語法（類似SQL指令的寫法）
            //===========================================
            if (String.IsNullOrEmpty(_SearchWord) && ModelState.IsValid)
            {
                // 沒有輸入內容，就會報錯                 // ModelState是「第五天」才要學的表單輸入驗證、Model Validation
                return Content("請輸入「關鍵字」才能搜尋");
            }

            var ListAll = from _userTable in _db.UserTables
                where _userTable.UserName.Contains(_SearchWord)
                // .Contains()對應T-SQL指令的 LIKE，但搜尋關鍵字有「大小寫」的區分
                // 如果想要排序（OrderBy）請參閱下一個範例。
                select _userTable;
            //ADO .NET 的寫法
            //            var ListAll = new List<UserTable>();
            //            string connectionString = "YourMySQLConnectionStringHere"; // 請替換為實際的 MySQL 連接字串
            //            string query = @"
            //SELECT UserId, UserName, UserSex, UserBirthDay, UserMobilePhone
            //FROM UserTables
            //WHERE UserName LIKE @SearchWord";
            //            using (MySqlConnection connection = new MySqlConnection(connectionString))
            //            {
            //                MySqlCommand command = new MySqlCommand(query, connection);
            //                command.Parameters.AddWithValue("@SearchWord", "%" + _SearchWord + "%");
            //                connection.Open();
            //                using MySqlDataReader reader = command.ExecuteReader();
            //                while (reader.Read())
            //                {
            //                    var user = new UserTable
            //                    {
            //                        UserId = reader.GetInt32(0),
            //                        UserName = reader.IsDBNull(1) ? null : reader.GetString(1),
            //                        UserSex = reader.IsDBNull(2) ? null : reader.GetString(2),
            //                        UserBirthDay = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
            //                        UserMobilePhone = reader.IsDBNull(4) ? null : reader.GetString(4)
            //                    };
            //                    ListAll.Add(user);
            //                }
            //            }
            ////*** 查詢結果 ListAll 是一個 IQueryable ***
            ////if (ListAll == null) // 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）。所以這段if辨別不了」
            if (ListAll.Any() == false) // 可運作
                ////if (ListAll.GetEnumerator().MoveNext() == false)   // 可運作
            {
                // 找不到任何記錄。 .NET Core改用 NotFound();
                return Content(" 找不到這一筆記錄！"); // 或寫成 return NotFound();
            }
            else
            {
                return View(ListAll);
                //檢視畫面的「範本」請選 List。因為搜尋到的結果可能會有多筆記錄。
            }
            //===========================================
            //第二種寫法：  Method Syntax，方法語法
            //===========================================
            //IQueryable<UserTable> ListAll = (from _userTable in _db.UserTables
            //                                                      select _userTable).AsNoTracking();
            //// .AsNoTracking()需搭配 System.Data.Entity命名空間。 https://dotblogs.com.tw/wasichris/2015/03/29/150868
            //// 避免使用快取（緩存、cache）的數據，直接查詢DB內 "最新" 資料。請勿搭配 .SaveChange()使用。
            //// 優點：可以查詢到最新資料。  缺點：沒有快取，速度會慢一點點。
            //if (!String.IsNullOrEmpty(_SearchWord) && ModelState.IsValid)
            //{
            //    return View(ListAll.Where(s => s.UserName.Contains(_SearchWord)));
            //    // 討論串  https://stackoverflow.com/questions/29824798/need-help-understanding-linq-in-mvc/29825045#29825045
            //    // 原廠文件（中文） https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-filtering
            //}
            //else
            //{   // 找不到任何記錄。 .NET Core改用 NotFound();
            //    return Content(" 找不到這一筆記錄！");     // 或寫成 return NotFound();
            //}
        }

        //=================================
        // .NET Core 6.0~8.0，我想用 TagHelper的 Create範本，改寫搜尋關鍵字的畫面（表單）
        //=================================
        public IActionResult SearchIndex()
        {
            // 空白的畫面與動作。 自己寫一個搜尋「關鍵字」的輸入畫面。
            return View();
        }

        // 上面第一個搜尋的動作，採用POST，所以URL輸入 http://xxxxxx/Home/Search?_SearchWord=MVC  會報錯
        // 下面的搜尋針對 Route做調整，加入一列程式碼，可以正確執行。
        // 正確執行。直接在網址輸入 http://xxxxxx/Home/Search2?_ID=MVC
        // 正確執行。直接在網址輸入 http://xxxxxx/Home/Search2/MVC
        //[HttpPost]   // 重點(1)！這一列務必註解、不執行！
        //[ValidateAntiForgeryToken]   // 避免XSS、CSRF攻擊，需搭配 List檢視畫面下方的輸入表格
        public ActionResult
            Search2(string _ID) // 重點(2)！！因為目前的路由設定，只能接受 id這個變數 （需要修改 Program.cs的  Route設定。把 id 改 _ID 才行）
        {
            string _SearchWord = _ID; // 重點(3)！！
            ViewData["SW"] = _SearchWord;
            ////第二種寫法：查詢語法
            //var ListAll = from _userTable in _db.UserTables
            //                      select _userTable;
            IQueryable<UserTable> ListAll = (from _userTable in _db.UserTables
                select _userTable).AsNoTracking();
            // .AsNoTracking()需搭配 System.Data.Entity命名空間。 https://dotblogs.com.tw/wasichris/2015/03/29/150868
            // 避免使用快取（緩存、cache）的數據，直接查詢DB內最新資料。請勿搭配 .SaveChange()使用。
            // 優點：可以查詢到最新資料。  缺點：沒有快取，速度會慢一點點。
            if (!String.IsNullOrEmpty(_SearchWord) && ModelState.IsValid)
            {
                return View(ListAll.Where(s => s.UserName.Contains(_SearchWord)));
                // .Where() 與 .Contains()的寫法 https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-filtering
            }
            else
            {
                // 找不到任何記錄。 .NET Core改用 NotFound();
                return Content(" 找不到這一筆記錄！"); // 或寫成 return NotFound();
            }
        }

        // 上面第一個搜尋的動作，採用POST，所以URL輸入 http://xxxxxx/Home/Search?_SearchWord=MVC  會報錯。本範例改成 HttpGet就OK。
        // 輸入這樣也是錯誤   http://xxxxxx/Home/Search3/MVC   （需要修改 Program.cs下的  Route設定。把 id 改 _ID 才行）
        // 下面的搜尋採用 GET，可以正確執行。http://xxxxxx/Home/Search3?_SearchWord=MVC
        [HttpGet]
        //[ValidateAntiForgeryToken]   // 避免XSS、CSRF攻擊，需搭配 List檢視畫面下方的輸入表格
        public ActionResult Search3(string _SearchWord = "MVC")
        {
            ViewData["SW"] = _SearchWord; // 如果您寫成 ViewBag也可以
            ////第二種寫法：查詢語法
            //var ListAll = from _userTable in _db.UserTables
            //                      select _userTable;
            IQueryable<UserTable> ListAll = (from _userTable in _db.UserTables
                select _userTable).AsNoTracking();
            // .AsNoTracking()需搭配 System.Data.Entity命名空間。 https://dotblogs.com.tw/wasichris/2015/03/29/150868
            // 避免使用快取（緩存、cache）的數據，直接查詢DB內最新資料。請勿搭配 .SaveChange()使用。
            // 優點：可以查詢到最新資料。  缺點：沒有快取，速度會慢一點點。
            if (!String.IsNullOrEmpty(_SearchWord) && ModelState.IsValid)
            {
                return View(ListAll.Where(s => s.UserName.Contains(_SearchWord)));
                // .Where() 與 .Contains()的寫法 https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-filtering
            }
            else
            {
                // 找不到任何記錄。 .NET Core改用 NotFound();
                return Content(" 找不到這一筆記錄！"); // 或寫成 return NotFound();
            }
        }

        //===================================
        //== 搜尋  「多個」關鍵字。  畫面上有「多個」搜尋條件。
        //== 中文範例   https://www.blueshop.com.tw/board/show.asp?subcde=BRD2012090415385840A&fumcde=FUM20050124192253INM&page=2
        //===================================
        public ActionResult Search4_Multi()
        {
            return View(); //產生一個搜尋畫面。類似「新增 Create」的畫面。  可以輸入多個搜尋條件。
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // 避免CSRF攻擊
        public ActionResult Search4_Multi(UserTable _userTable)
        {
            //                                                 ******************** 修改的重點！
            //                                                 輸入多個關鍵字，用 ViewModel包起來一起傳回來
            string uName = _userTable.UserName; // 從畫面上，輸入的第一個搜尋條件。  姓名。
            string uMobilePhone = _userTable.UserMobilePhone; // 從畫面上，輸入的第二個搜尋條件。   手機號碼。
            //#region ////** 作法一 ************************************************
            //// 「全部的」搜尋條件都要輸入，一個都不能少，才能找得到結果 （這種作法不是我們想要的）
            //var ListAll = from _uTable in _db.UserTables
            //                    where _uTable.UserName == uName && _uTable.UserMobilePhone == uMobilePhone
            //                    select _userTable;
            ////// 翻譯後的SQL指令變成這樣。關鍵字不是模糊搜尋，是等號（=）.......精準搜尋！！
            ////// SELECT *
            //////  FROM[UserTable] AS[u]
            //////  WHERE[u].[UserName] = 'asp' AND[u].[UserMobilePhone] = '0988'
            ////**********************************************************
            //#endregion
            ////*** 查詢結果 ListAll 是一個 IQueryable ***
            ////if ((_userTable != null) && (ModelState.IsValid))  // 有問題。// 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）」。所以這段if辨別不了
            ////if ((_userTable.Any() != false) && (ModelState.IsValid))
            //{
            //    return View("Search4_Result", ListAll.ToList());
            //    // 搜尋結果（ListAll），導向另一個「檢視畫面（Search4_Result）」！
            //    // .Where() 與 .Contains()的寫法 https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/ef/language-reference/method-based-query-syntax-examples-filtering
            //}
            //else
            //{   // 找不到任何記錄。 .NET Core改用 NotFound();
            //    return Content(" 找不到這一筆記錄！");     // 或寫成 return NotFound();
            //}

            #region // ** 作法二 ************************************************(start)

            var ListAll = _db.UserTables.Select(s => s);
            if (!string.IsNullOrWhiteSpace(uName)) // 「有填寫」搜尋條件的，才會進行搜尋。
            {
                //  畫面上留空白（不輸入），表示這個條件不搜尋。
                //ListAll = ListAll.Where(s => s.UserName == uName);
                ListAll = ListAll.Where(s => s.UserName.Contains(uName));
                //                                                         // ********** 模糊搜尋，類似SQL指令的 Like '%'
            }

            if (!string.IsNullOrWhiteSpace(uMobilePhone))
            {
                ListAll = ListAll.Where(s => s.UserMobilePhone.Contains(uMobilePhone));
                //                                                                      // ********** 模糊搜尋，類似SQL指令的 Like '%'
            }

            //// ADO .NET 的寫法
            //var ListAll = new List<UserTable>();
            //var query = "SELECT * FROM UserTable WHERE 1=1";
            //if (!string.IsNullOrWhiteSpace(uName))
            //{
            //    query += " AND UserName LIKE @UserName";
            //}
            //if (!string.IsNullOrWhiteSpace(uMobilePhone))
            //{
            //    query += " AND UserMobilePhone LIKE @UserMobilePhone";
            //}
            //using (var connection = new MySqlConnection("YourConnectionStringHere"))
            //{
            //    using var command = new MySqlCommand(query, connection);
            //    if (!string.IsNullOrWhiteSpace(uName))
            //    {
            //        command.Parameters.AddWithValue("@UserName", "%" + uName + "%");
            //    }
            //    if (!string.IsNullOrWhiteSpace(uMobilePhone))
            //    {
            //        command.Parameters.AddWithValue("@UserMobilePhone", "%" + uMobilePhone + "%");
            //    }
            //    connection.Open();
            //    using var reader = command.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        var user = new UserTable
            //        {
            //            UserId = reader.GetInt32("UserId"),
            //            UserName = reader.GetString("UserName"),
            //            UserSex = reader.GetString("UserSex"),
            //            UserBirthDay = reader.GetDateTime("UserBirthDay"),
            //            UserMobilePhone = reader.GetString("UserMobilePhone")
            //        };
            //        ListAll.Add(user);
            //    }
            //}

            #endregion //**********************************************************(end)

            ////if ((ListAll != null) && (ModelState.IsValid))  // 有問題。// 查無資料時，無法正確運作。因為 IQueryable<T>會傳回一個「空集合」而不是「空（null）」。所以這段if辨別不了
            if ((ListAll.Any() != false) && (ModelState.IsValid))
            {
                return View("Search4_Result", ListAll);
                // 搜尋結果（ListAll），導向另一個「檢視畫面（Search4_Result）」！
            }
            else
            {
                // 找不到任何記錄。 .NET Core改用 NotFound();
                return Content(" 找不到這一筆記錄！"); // 或寫成 return NotFound();
            }
        }
    }
}