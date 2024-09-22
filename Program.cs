using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;     // �Цۤv��ʦw��NuGet -- ConfigurationBuilder�|�Ψ�o�өR�W�Ŷ�
using Microsoft.Extensions.DependencyInjection;
using WebApplication2022_NetCore8_MVC.Models;  // �Цۤv��ʥ[�J�o�өR�W�Ŷ��C
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
// �ۤv�ʤ�[�J���q��  (DI)
#region
//**** Ū�� appsettings.json �]�w�ɸ̭�����ơ]��Ʈw�s���r��^****
////�@�k�@�G
//builder.Services.AddDbContext<MvcUserDbContext>(options => options.UseSqlServer("Server=.\\sqlexpress;Database=MVC_UserDB;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true"));
//// �o�̻ݭn�s�W��өR�W�Ŷ��A�ШϥΡu��ܥi�઺�ץ��v���t�Φۤv�[�W�C
////�@�k�G�G Ū���]�w�ɪ����e
//// ��ƨӷ�  �{���X  https://github.com/CuriousDrive/EFCore.AllDatabasesConsidered/blob/main/MS%20SQL%20Server/Northwind.MSSQL/Program.cs
builder.Services.AddDbContext<MvcUserDbContext>(
        options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
////�@�k�T�G Ū���]�w�ɪ����e
////****  ��Ʈw�s���r�� **** Ū�� appsettings.json �]�w�ɸ̭�����ơ]��Ʈw�s���r��^****
//// (3-A) �i�B�@
//var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
//IConfiguration config = configurationBuilder.Build();   // ConfigurationBuilder�|�Ψ� Microsoft.Extensions.Configuration�R�W�Ŷ�
//builder.Services.AddDbContext<MVC_UserDB2Context>(options => options.UseSqlServer(config["ConnectionStrings:DefaultConnection"]));  // appsettings.josn�ɸ̭����u��Ʈw�s���r��v
//// (3-B) �i�B�@
//IConfiguration config = builder.Configuration;   // ConfigurationBuilder�|�Ψ� Microsoft.Extensions.Configuration�R�W�Ŷ�
//builder.Services.AddDbContext<MVC_UserDB2Context>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));  // appsettings.josn�ɸ̭����u��Ʈw�s���r��v
//======================
//�ϥΧ����A�|"�۰�"������Ʈw�s�u�C https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud?view=aspnetcore-5.0
//======================
// �z�|�I�s .AddDbContext() �X�R��k �Ӧb ASP.NET Core DI �e�����G�� DbContext ���O�C
// �ھڹw�]�A�Ӥ�k�|�N�A�Ȧs�d���]�w�� Scoped�C
// Scoped ��ܤ��e���󪺦s�d���|�P Web �n�D���s�d���O���@�P�A�åB�b Web �n�D�����ɷ|�۰ʩI�s Dispose ��k�C
//********************************************************************
//**** Ū�� appsettings.json �]�w�ɸ̭�����ơ]��Ʈw�s���r��^for ADO.NET�ϥΨê`�JDI ****
// Ū�� appsettings.json �]�w�ɸ̭�����ơ]��Ʈw�s���r��^
IConfiguration config = builder.Configuration;
string connectionString = config.GetConnectionString("DefaultConnection");
// �N�s�u�r��`�J DI �e��
builder.Services.AddSingleton(connectionString);
//********************************************************************
#endregion
//==== �� �j �u =============================
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