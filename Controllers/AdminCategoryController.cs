using Microsoft.AspNetCore.Mvc;

namespace YourProjectName.Controllers
{
    public class AdminCategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}