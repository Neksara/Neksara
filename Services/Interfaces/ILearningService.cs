using Neksara.Models;
using Neksara.ViewModels;

namespace Neksara.Services
{
    public interface ILearningService
    {
        // CATEGORY
        Task<List<CategoryCardVM>> GetCategoryCardsAsync();

        // TOPIC
        Task<TopicListVM> GetTopicCardsAsync(int? categoryId, int page, int pageSize);

        // DETAIL
        Task<Topic?> GetTopicDetailAsync(int topicId);
        Task IncrementViewCountAsync(Topic topic);
        Task<double> GetAverageRatingAsync(int topicId);
    }
}
