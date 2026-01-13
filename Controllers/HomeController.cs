using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Neksara.Models;
using Neksara.Models.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Neksara.Services;
using Neksara.Services.Interfaces;

namespace Neksara.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly ILearningService _learning_service;
        private readonly ITestimoniService _testimoni_service;
        private readonly IMemoryCache _cache;

        public HomeController(
            ILogger<HomeController> logger,
            ICategoryService categoryService,
            ILearningService learningService,
            ITestimoniService testimoniService,
            IMemoryCache cache)
        {
            _logger = logger;
            _categoryService = categoryService;
            _learning_service = learningService;
            _testimoni_service = testimoniService;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.PopularCategories = await _categoryService.GetPopularAsync();

            const int take = 8;
            var cacheKey = $"PopularTopics_take_{take}";

            if (!_cache.TryGetValue<List<Neksara.ViewModels.TopicCardVM>>(cacheKey, out var popular))
            {
                popular = await _learning_service.GetPopularTopicsAsync(take);
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                _cache.Set(cacheKey, popular, cacheOptions);
            }

            ViewBag.PopularTopics = popular;

            ViewBag.Testimonials = await _testimoni_service.GetPublishedAsync(null);

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
