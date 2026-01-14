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

    Task<List<Topic>> GetAllAsync(string? search, string? sort, int? categoryId);
}
