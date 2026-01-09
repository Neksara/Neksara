using Microsoft.AspNetCore.Mvc;

namespace YourProjectName.Controllers
{
    public class AdminArchivedPagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}