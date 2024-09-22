using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;     // 請自己手動安裝NuGet -- ConfigurationBuilder會用到這個命名空間
using Microsoft.Extensions.DependencyInjection;
using WebApplication2022_NetCore8_MVC.Models;  // 請自己手動加入這個命名空間。
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
// 自己動手加入的段落  (DI)
#region
//**** 讀取 appsettings.json 設定檔裡面的資料（資料庫連結字串）****
////作法一：
//builder.Services.AddDbContext<MvcUserDbContext>(options => options.UseSqlServer("Server=.\\sqlexpress;Database=MVC_UserDB;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true"));
//// 這裡需要新增兩個命名空間，請使用「顯示可能的修正」讓系統自己加上。
////作法二： 讀取設定檔的內容
//// 資料來源  程式碼  https://github.com/CuriousDrive/EFCore.AllDatabasesConsidered/blob/main/MS%20SQL%20Server/Northwind.MSSQL/Program.cs
builder.Services.AddDbContext<MvcUserDbContext>(
        options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
////作法三： 讀取設定檔的內容
////****  資料庫連結字串 **** 讀取 appsettings.json 設定檔裡面的資料（資料庫連結字串）****
//// (3-A) 可運作
//var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
//IConfiguration config = configurationBuilder.Build();   // ConfigurationBuilder會用到 Microsoft.Extensions.Configuration命名空間
//builder.Services.AddDbContext<MVC_UserDB2Context>(options => options.UseSqlServer(config["ConnectionStrings:DefaultConnection"]));  // appsettings.josn檔裡面的「資料庫連結字串」
//// (3-B) 可運作
//IConfiguration config = builder.Configuration;   // ConfigurationBuilder會用到 Microsoft.Extensions.Configuration命名空間
//builder.Services.AddDbContext<MVC_UserDB2Context>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));  // appsettings.josn檔裡面的「資料庫連結字串」
//======================
//使用完成，會"自動"關閉資料庫連線。 https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud?view=aspnetcore-5.0
//======================
// 您會呼叫 .AddDbContext() 擴充方法 來在 ASP.NET Core DI 容器中佈建 DbContext 類別。
// 根據預設，該方法會將服務存留期設定為 Scoped。
// Scoped 表示內容物件的存留期會與 Web 要求的存留期保持一致，並且在 Web 要求結束時會自動呼叫 Dispose 方法。
//********************************************************************
//**** 讀取 appsettings.json 設定檔裡面的資料（資料庫連結字串）for ADO.NET使用並注入DI ****
// 讀取 appsettings.json 設定檔裡面的資料（資料庫連結字串）
IConfiguration config = builder.Configuration;
string connectionString = config.GetConnectionString("DefaultConnection");
// 將連線字串注入 DI 容器
builder.Services.AddSingleton(connectionString);
//********************************************************************
#endregion
//==== 分 隔 線 =============================
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();