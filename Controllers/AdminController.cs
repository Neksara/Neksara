using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neksara.Models;
using Neksara.Services.Interfaces;

namespace Neksara.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ITopicService _topicService;

        private const int PAGE_SIZE = 5;

        public AdminController(
            ICategoryService categoryService,
            ITopicService topicService)
        {
            _categoryService = categoryService;
            _topicService = topicService;
        }

        // =========================
        // DASHBOARD
        // =========================
        public IActionResult Index()
        {
            return RedirectToAction(nameof(AdminPanel));
        }

        public IActionResult AdminPanel()
        {
            return View();
        }

        // =========================
        // CATEGORY
        // =========================
        public async Task<IActionResult> CategoryIndex(string search, int page = 1)
        {
            var (data, totalData) =
                await _categoryService.GetCategorySummaryAsync(
                    search, page, PAGE_SIZE);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling(totalData / (double)PAGE_SIZE);
            ViewBag.Search = search;

            return View(data);
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.CreateAsync(model);
            return RedirectToAction(nameof(CategoryIndex));
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var data = await _categoryService.GetByIdAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.UpdateAsync(model);
            return RedirectToAction(nameof(CategoryIndex));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.SoftDeleteAsync(id);
            return RedirectToAction(nameof(CategoryIndex));
        }

        // =========================
        // TOPIC
        // =========================
        public async Task<IActionResult> TopicIndex(string search, int page = 1)
        {
            var (data, totalData) =
                await _topicService.GetPagedAsync(search, page, PAGE_SIZE);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling(totalData / (double)PAGE_SIZE);
            ViewBag.Search = search;

            return View(data);
        }

        public async Task<IActionResult> CreateTopic()
        {
            ViewBag.Categories = await _topicService.GetCategoriesAsync();
            return View(new Topic());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic(Topic model, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _topicService.GetCategoriesAsync();
                return View(model);
            }

            await _topicService.CreateAsync(model, image);
            return RedirectToAction(nameof(TopicIndex));
        }

        public async Task<IActionResult> EditTopic(int id)
        {
            var data = await _topicService.GetByIdAsync(id);
            if (data == null) return NotFound();

            ViewBag.Categories = await _topicService.GetCategoriesAsync();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditTopic(
            Topic model,
            IFormFile? image,
            string ExistingPicture)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _topicService.GetCategoriesAsync();
                return View(model);
            }

            await _topicService.UpdateAsync(model, image, ExistingPicture);
            return RedirectToAction(nameof(TopicIndex));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            await _topicService.ArchiveAsync(id);
            return RedirectToAction(nameof(TopicIndex));
        }

        public async Task<IActionResult> CategoryDetail(int id)
        {
            var data = await _categoryService.GetDetailAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }

        public async Task<IActionResult> TopicDetail(int id)
        {
            var data = await _topicService.GetDetailAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }

    }
}
