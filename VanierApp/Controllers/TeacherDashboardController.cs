using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VanierApp.Models;

namespace VanierApp.Controllers
{
    public class TeacherDashboardController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
