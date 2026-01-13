using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Neksara.Models;
using Neksara.Models.ViewModels;
using Neksara.Services;
using Neksara.Services.Interfaces;

namespace Neksara.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly ILearningService _learningService;
        private readonly ITestimoniService _testimoniService;

        public HomeController(
            ILogger<HomeController> logger,
            ICategoryService categoryService,
            ILearningService learningService,
            ITestimoniService testimoniService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _learningService = learningService;
            _testimoniService = testimoniService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PopularCategories = await _categoryService.GetPopularAsync();

            ViewBag.PopularTopics = await _learningService.GetPopularTopicsAsync(8);

            ViewBag.Testimonials = await _testimoniService.GetPublishedAsync(null);

            return View();
        }

        public IActionResult Learning()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
