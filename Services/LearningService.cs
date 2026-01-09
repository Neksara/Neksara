using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;

namespace Neksara.Services
{
    public class LearningService : ILearningService
    {
        private readonly ApplicationDbContext _context;

        public LearningService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<(List<Topic> Topics, int TotalPages)> GetTopicsAsync(int? categoryId, int page, int pageSize)
        {
            var query = _context.Topics
                .Include(t => t.Category)
                .Include(t => t.Feedbacks)
                .Where(t => !t.IsDeleted && t.PublishedAt != default)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var topics = await query
                .OrderByDescending(t => t.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (topics, totalPages);
        }

        public async Task<Topic?> GetTopicDetailAsync(int topicId)
        {
            return await _context.Topics
                .Include(t => t.Category)
                .Include(t => t.Feedbacks)
                .FirstOrDefaultAsync(t => t.TopicId == topicId &&
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
                .Where(f => f.TargetType == "Topic" &&
                            f.TargetId == topicId &&
                            f.IsApproved &&
                            f.IsVisible)
                .AverageAsync(f => (double?)f.Rating) ?? 0;

            return Math.Round(avg, 1);
        }
    }
}
