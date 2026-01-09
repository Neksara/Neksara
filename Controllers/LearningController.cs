using Microsoft.AspNetCore.Mvc;
using Neksara.Models;
using Neksara.Services;

namespace Neksara.Controllers
{
    public class LearningController : Controller
    {
        private readonly ILearningService _service;
        private const int PageSize = 6;

        public LearningController(ILearningService service)
        {
            _service = service;
        }

        // ================= LIST + FILTER + PAGINATION =================
        public async Task<IActionResult> Index(int? categoryId, int page = 1)
        {
            var categories = await _service.GetCategoriesAsync();
            var (topics, totalPages) = await _service.GetTopicsAsync(categoryId, page, PageSize);

            // Hitung rating per topik
            var ratingsDict = topics.ToDictionary(
                t => t.TopicId,
                t => t.Feedbacks != null && t.Feedbacks.Any()
                     ? t.Feedbacks.Average(f => f.Rating)
                     : 0.0
            );

            ViewBag.Category = categories;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Ratings = ratingsDict;

            return View(topics);
        }

        // ================= DETAIL TOPIK =================
        public async Task<IActionResult> Detail(int id)
        {
            var topic = await _service.GetTopicDetailAsync(id);
            if (topic == null) return NotFound();

            await _service.IncrementViewCountAsync(topic);
            ViewBag.AvgRating = await _service.GetAverageRatingAsync(id);

            return View(topic);
        }
    }
}
