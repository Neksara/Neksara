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
        public async Task<IActionResult> CategoryIndex(
            string? search,
            string? sort,
            int page = 1)
        {
            var result = await _categoryService.GetCategoryIndexAsync(
                search,
                sort,
                page,
                PAGE_SIZE
            );

            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.TotalPage;

            return View(result.Items);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category model, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.CreateAsync(model, image);
            return RedirectToAction(nameof(CategoryIndex));
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var data = await _categoryService.GetByIdAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category model, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.UpdateAsync(model, image);
            return RedirectToAction(nameof(CategoryIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.SoftDeleteAsync(id);
            return RedirectToAction(nameof(CategoryIndex));
        }

        public async Task<IActionResult> CategoryDetail(int id)
        {
            var data = await _categoryService.GetDetailAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }

        // =========================
        // TOPIC
        // =========================
        public async Task<IActionResult> TopicIndex(
            string? search,
            string? sort,
            int? categoryId)
        {
            // 🔥 UPDATED: Get ALL topics tanpa paging
            var topics = await _topicService.GetAllAsync(search, sort, categoryId);

            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await _topicService.GetCategoriesAsync();

            return View(topics);
        }

        // ---------- CREATE ----------
        [HttpGet]
        public async Task<IActionResult> CreateTopic()
        {
            ViewBag.Categories = await _topicService.GetCategoriesAsync();
            return View(new Topic());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // ---------- EDIT ----------
        [HttpGet]
        public async Task<IActionResult> EditTopic(int id)
        {
            var data = await _topicService.GetByIdAsync(id);
            if (data == null) return NotFound();

            ViewBag.Categories = await _topicService.GetCategoriesAsync();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTopic(
            Topic model,
            IFormFile? image,
            string? ExistingPicture)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _topicService.GetCategoriesAsync();
                return View(model);
            }

            model.UpdatedAt = DateTime.Now;
            model.PublishedAt = null; // NON ACTIVE

            await _topicService.UpdateAsync(model, image, ExistingPicture);
            return RedirectToAction(nameof(TopicIndex));
        }

        // ---------- DELETE ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            await _topicService.ArchiveAsync(id);
            return RedirectToAction(nameof(TopicIndex));
        }

        // ---------- DETAIL ----------
        public async Task<IActionResult> TopicDetail(int id)
        {
            var data = await _topicService.GetDetailAsync(id);
            if (data == null) return NotFound();

            return View(data);
        }
    }
}
