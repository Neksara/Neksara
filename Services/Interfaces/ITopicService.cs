    using Neksara.Models;
    using Neksara.ViewModels;

    namespace Neksara.Services.Interfaces;
    public interface ITopicService
    {
        Task<(List<Topic>, int)> GetPagedAsync(string search, int page, int pageSize);
        Task<List<Category>> GetCategoriesAsync();
        Task<TopicDetailVM?> GetDetailAsync(int topicId);
        Task CreateAsync(Topic model, IFormFile? image);
        Task<Topic?> GetByIdAsync(int id);
        Task UpdateAsync(Topic model, IFormFile? image, string existingPicture);
        Task ArchiveAsync(int id);
        Task HardDeleteArchiveAsync(int archiveTopicId);
    }
