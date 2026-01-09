using Microsoft.AspNetCore.Mvc;

namespace YourProjectName.Controllers
{
    public class AdminTopicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}