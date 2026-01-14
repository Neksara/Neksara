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
                    TotalTopics = c.Topics
                        .Count(t => !t.IsDeleted && t.PublishedAt != null)
                })
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        // ================= TOPIC CARD =================
        public async Task<TopicListVM> GetTopicCardsAsync(
            int? categoryId, int page, int pageSize)
        {
            var query = _context.Topics
                .Where(t => !t.IsDeleted && t.PublishedAt != null);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId);

            int total = await query.CountAsync();

            var topics = await query
                .OrderByDescending(t => t.ViewCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TopicCardVM
                {
                    TopicId = t.TopicId,
                    TopicName = t.TopicName,
                    TopicPicture = t.TopicPicture,
                    CategoryName = t.Category!.CategoryName,
                    ViewCount = t.ViewCount,
                    PublishedAt = t.PublishedAt,

                    // 💬 JUMLAH REVIEW (FIX)
                    ReviewCount = _context.Feedbacks.Count(f =>
                        f.TargetType == "Topic" &&
                        f.TargetId == t.TopicId &&
                        f.IsApproved &&
                        f.IsVisible
                    ),

                    // ⭐ RATING RATA-RATA (FIX)
                    Rating = _context.Feedbacks
                        .Where(f =>
                            f.TargetType == "Topic" &&
                            f.TargetId == t.TopicId &&
                            f.IsApproved &&
                            f.IsVisible
                        )
                        .Any()
                            ? _context.Feedbacks
                                .Where(f =>
                                    f.TargetType == "Topic" &&
                                    f.TargetId == t.TopicId &&
                                    f.IsApproved &&
                                    f.IsVisible
                                )
                                .Average(f => f.Rating)
                            : 0
                })
                .ToListAsync();

            return new TopicListVM
            {
                Topics = topics,
                CurrentCategory = categoryId,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        // ================= DETAIL =================
        public async Task<Topic?> GetTopicDetailAsync(int topicId)
        {
            return await _context.Topics
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t =>
                    t.TopicId == topicId &&
                    !t.IsDeleted &&
                    t.PublishedAt != null);
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
        public async Task<List<TopicCardVM>> GetPopularTopicsAsync(int take)
        {
            return await _context.Topics
                .Where(t => !t.IsDeleted && t.PublishedAt != null)
                .OrderByDescending(t => t.ViewCount)
                .Take(take)
                .Select(t => new TopicCardVM
                {
                    TopicId = t.TopicId,
                    TopicName = t.TopicName,
                    TopicPicture = t.TopicPicture,
                    CategoryName = t.Category!.CategoryName,
                    ViewCount = t.ViewCount,
                    PublishedAt = t.PublishedAt,

                    ReviewCount = _context.Feedbacks.Count(f =>
                        f.TargetType == "Topic" &&
                        f.TargetId == t.TopicId &&
                        f.IsApproved &&
                        f.IsVisible
                    ),

                    Rating = _context.Feedbacks
                        .Where(f =>
                            f.TargetType == "Topic" &&
                            f.TargetId == t.TopicId &&
                            f.IsApproved &&
                            f.IsVisible
                        )
                        .Any()
                            ? _context.Feedbacks
                                .Where(f =>
                                    f.TargetType == "Topic" &&
                                    f.TargetId == t.TopicId &&
                                    f.IsApproved &&
                                    f.IsVisible
                                )
                                .Average(f => f.Rating)
                            : 0
                })
                .ToListAsync();
        }
        // ================= FEEDBACK YANG DITAMPILKAN =================
public async Task<List<FeedbackVM>> GetVisibleFeedbacksAsync(int topicId)
{
    return await _context.Feedbacks
        .Where(f =>
            f.TargetType == "Topic" &&
            f.TargetId == topicId &&
            f.IsApproved &&
            f.IsVisible // 🔥 HANYA APPROVE + SHOW
        )
        .OrderByDescending(f => f.CreatedAt)
        .Select(f => new FeedbackVM
        {
            Name = f.Name,
            Role = f.Role,
            Rating = f.Rating,
            Description = f.Description
        })
        .ToListAsync();
}

// ================= TOTAL REVIEWER (APPROVE + HIDDEN) =================
public async Task<int> GetTotalReviewerAsync(int topicId)
{
    return await _context.Feedbacks.CountAsync(f =>
        f.TargetType == "Topic" &&
        f.TargetId == topicId &&
        f.IsApproved // 🔥 HIDDEN TETAP KEHITUNG
    );
}

    }
    
}
