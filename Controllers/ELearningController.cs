using Microsoft.AspNetCore.Mvc;

namespace Neksara.Controllers
{
  public class ELearningController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}