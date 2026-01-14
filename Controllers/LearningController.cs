using Microsoft.AspNetCore.Mvc;
using Neksara.Services;

namespace Neksara.Controllers
{
    public class LearningController : Controller
    {
        private readonly ILearningService _service;
        private const int PageSize = 8;

        public LearningController(ILearningService service)
        {
            _service = service;
        }

        // ================= KATEGORI PAGE =================
        public async Task<IActionResult> Categories(int? categoryId)
        {
            var categories = await _service.GetCategoryCardsAsync();

            if (categoryId.HasValue)
            {
                categories = categories
                    .Where(c => c.CategoryId == categoryId.Value)
                    .ToList();
            }

            ViewBag.SelectedCategoryId = categoryId;

            return View(categories);
        }

        // ================= TOPIC PAGE =================
        public async Task<IActionResult> Topics(int? categoryId, int page = 1)
        {
            var vm = await _service.GetTopicCardsAsync(categoryId, page, PageSize);
            return View(vm);
        }

        // ================= DETAIL TOPIC =================
        public async Task<IActionResult> Detail(int id)
        {
            var topic = await _service.GetTopicDetailAsync(id);
            if (topic == null) return NotFound();

            await _service.IncrementViewCountAsync(topic);

            ViewBag.AvgRating = await _service.GetAverageRatingAsync(id);
            ViewBag.TotalReviewer = await _service.GetTotalReviewerAsync(id);
            ViewBag.Feedbacks = await _service.GetVisibleFeedbacksAsync(id);

            return View(topic);
        }
    }
}
