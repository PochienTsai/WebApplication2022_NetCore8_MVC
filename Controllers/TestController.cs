using Microsoft.AspNetCore.Mvc;

namespace WebApplication2022_NetCore8_MVC.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
