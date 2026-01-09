using Microsoft.AspNetCore.Mvc;

namespace Neksara.Controllers
{
  public class AdminController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}