using Microsoft.AspNetCore.Mvc;

namespace Neksara.Controllers
{
  public class AdminFeedbackController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}