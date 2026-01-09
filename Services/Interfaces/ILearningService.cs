using Neksara.Models;

namespace Neksara.Services
{
    public interface ILearningService
    {
        Task<(List<Topic> Topics, int TotalPages)> GetTopicsAsync(int? categoryId, int page, int pageSize);
        Task<double> GetAverageRatingAsync(int topicId);
        Task<Topic?> GetTopicDetailAsync(int topicId);
        Task IncrementViewCountAsync(Topic topic);
        Task<List<Category>> GetCategoriesAsync();
    }
}
