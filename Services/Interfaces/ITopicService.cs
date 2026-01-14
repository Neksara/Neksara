using Neksara.Models;
using Neksara.ViewModels;

namespace Neksara.Services.Interfaces;

using Neksara.Models;

public interface ITopicService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<Topic?> GetByIdAsync(int id);
    Task CreateAsync(Topic model, IFormFile? image);
    Task UpdateAsync(Topic model, IFormFile? image, string? existingPicture);
    Task ArchiveAsync(int id);
    Task<TopicDetailVM?> GetDetailAsync(int topicId);

    // Get topics for admin index with paging
    Task<(List<Topic> Items, int TotalPage)> GetAllAsync(string? search, string? sort, int page, int pageSize, int? categoryId);
}
