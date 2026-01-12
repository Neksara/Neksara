using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;
using Neksara.ViewModels;

namespace Neksara.Services
{
    public class LearningService : ILearningService
    {
        private readonly ApplicationDbContext _context;

        public LearningService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= CATEGORY CARD =================
        public async Task<List<CategoryCardVM>> GetCategoryCardsAsync()
        {
            return await _context.Categories
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryCardVM
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryPicture = c.CategoryPicture,
                    Description = c.Description,
                    TotalTopics = c.Topics.Count(t => !t.IsDeleted && t.PublishedAt != default)
                })
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // ================= TOPIC CARD =================
        public async Task<TopicListVM> GetTopicCardsAsync(int? categoryId, int page, int pageSize)
        {
            var query = _context.Topics
                .Include(t => t.Category)
                .Include(t => t.Feedbacks.Where(f => f.IsApproved && f.IsVisible))
                .Where(t => !t.IsDeleted && t.PublishedAt != default);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var topics = await query
                .OrderByDescending(t => t.ViewCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TopicCardVM
                {
                    TopicId = t.TopicId,
                    TopicName = t.TopicName,
                    TopicPicture = t.TopicPicture,
                    CategoryName = t.Category.CategoryName,
                    Rating = t.Feedbacks.Any()
                        ? Math.Round(t.Feedbacks.Average(f => f.Rating), 1)
                        : 0,
                    ReviewCount = t.Feedbacks.Count,
                    ViewCount = t.ViewCount
                })
                .ToListAsync();

            return new TopicListVM
            {
                Topics = topics,
                CurrentCategory = categoryId,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        // ================= DETAIL =================
        public async Task<Topic?> GetTopicDetailAsync(int topicId)
        {
            return await _context.Topics
                .Include(t => t.Category)
                .Include(t => t.Feedbacks.Where(f => f.IsApproved && f.IsVisible))
                .FirstOrDefaultAsync(t =>
                    t.TopicId == topicId &&
                    !t.IsDeleted &&
                    t.PublishedAt != default);
        }

        public async Task IncrementViewCountAsync(Topic topic)
        {
            topic.ViewCount++;
            await _context.SaveChangesAsync();
        }

        public async Task<double> GetAverageRatingAsync(int topicId)
        {
            var avg = await _context.Feedbacks
                .Where(f =>
                    f.TargetType == "Topic" &&
                    f.TargetId == topicId &&
                    f.IsApproved &&
                    f.IsVisible)
                .AverageAsync(f => (double?)f.Rating) ?? 0;

            return Math.Round(avg, 1);
        }
    }
}
