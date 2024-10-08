using Microsoft.AspNetCore.Mvc;

namespace VanierApp.Controllers
{
    public class ErrorController : Controller
    {

        public IActionResult Index()
        {
            ViewData["Title"] = "Error";
            return View();
        }
    }
}
