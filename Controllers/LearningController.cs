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
        private const int PageSize = 12;
        private const int CategoryPageSize = 12;
        private const int TopicListCacheSeconds = 60;

        [ActivatorUtilitiesConstructor]
        public LearningController(ILearningService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        // ================= KATEGORI PAGE =================
        public async Task<IActionResult> CategoryIndex(int? categoryId, int page = 1)
        {
            var categories = await _service.GetCategoryCardsAsync();

            if (categoryId.HasValue)
            {
                categories = categories
                    .Where(c => c.CategoryId == categoryId.Value)
                    .ToList();
            }

            var totalItems = categories.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)CategoryPageSize);
            
            // Ensure valid page
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedCategories = categories
                .Skip((page - 1) * CategoryPageSize)
                .Take(CategoryPageSize)
                .ToList();

            var vm = new CategoryListVM
            {
                Categories = pagedCategories,
                CurrentPage = page,
                TotalPages = totalPages,
                SelectedCategoryId = categoryId
            };

            ViewBag.SelectedCategoryId = categoryId;

            return View("Categories", vm);
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

        // Provide a default Index route that redirects to Topics
        public IActionResult Index(int? categoryId, int page = 1)
        {
            return RedirectToAction("Topics", new { categoryId = categoryId, page = page });
        }

        // ================= DETAIL TOPIC =================
        public async Task<IActionResult> Detail(int id)
        {
            var cacheKey = $"TopicDetail_{id}";

            if (!_cache.TryGetValue<Topic>(cacheKey, out var topic))
            {
                topic = await _service.GetTopicDetailAsync(id);
                if (topic == null) return NotFound();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60)); // Cache detail for 60s
                _cache.Set(cacheKey, topic, cacheOptions);
            }

            // Parallelize auxiliary data fetching
            var ratingTask = _service.GetAverageRatingAsync(id);
            var reviewersTask = _service.GetTotalReviewerAsync(id);
            var feedbacksTask = _service.GetVisibleFeedbacksAsync(id);

            await Task.WhenAll(ratingTask, reviewersTask, feedbacksTask);

            ViewBag.AvgRating = await ratingTask;
            ViewBag.TotalReviewer = await reviewersTask;
            ViewBag.Feedbacks = await feedbacksTask;

            await _service.IncrementViewCountAsync(topic);

            return View(topic);
        }
    }
}
