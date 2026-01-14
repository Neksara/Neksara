using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Neksara.Services;
using Neksara.ViewModels;

namespace Neksara.Controllers
{
    public class LearningController : Controller
    {
        private readonly ILearningService _service;
        private readonly IMemoryCache _cache;
        private const int PageSize = 8;
        private const int TopicListCacheSeconds = 60;

        [ActivatorUtilitiesConstructor]
        public LearningController(ILearningService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
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
            var cacheKey = $"TopicList_{(categoryId.HasValue ? categoryId.Value.ToString() : "all")}_page_{page}";

            if (!_cache.TryGetValue<TopicListVM>(cacheKey, out var vm))
            {
                vm = await _service.GetTopicCardsAsync(categoryId, page, PageSize);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(TopicListCacheSeconds));

                _cache.Set(cacheKey, vm, cacheOptions);
            }

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
