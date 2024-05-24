using Microsoft.AspNetCore.Mvc;

namespace DotNetBack.Controllers
{
    public class Message : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
