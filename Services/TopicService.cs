using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;
using Neksara.Services.Interfaces;
using Neksara.ViewModels;

namespace Neksara.Services;

public class TopicService : ITopicService
{
    private readonly ApplicationDbContext _context;

    public TopicService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Topic>> GetAllAsync(string? search, string? sort, int? categoryId)
    {
        var query = _context.Topics
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.TopicName.Contains(search) ||
                                     t.Category.CategoryName.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        query = sort switch
        {
            "az" => query.OrderBy(t => t.TopicName),
            "za" => query.OrderByDescending(t => t.TopicName),
            "views" => query.OrderByDescending(t => t.ViewCount),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        return await query.ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync()
        => await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();

    public async Task<TopicDetailVM?> GetDetailAsync(int topicId)
    {
        var topic = await _context.Topics
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.TopicId == topicId && !t.IsDeleted);

        if (topic == null) return null;

        return new TopicDetailVM
        {
            TopicId = topic.TopicId,
            TopicName = topic.TopicName,
            CategoryName = topic.Category!.CategoryName,
            TopicPicture = topic.TopicPicture,
            Description = topic.Description,
            VideoUrl = topic.VideoUrl,
            ViewCount = topic.ViewCount,
            CreatedAt = topic.CreatedAt
        };
    }

    public async Task CreateAsync(Topic model, IFormFile? image)
    {
        if (image != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(folder);

            using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await image.CopyToAsync(stream);

            model.TopicPicture = "/images/" + fileName;
        }

        model.CreatedAt = DateTime.Now;
        model.ViewCount = 0;
        model.IsDeleted = false;

        _context.Topics.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task<Topic?> GetByIdAsync(int id)
        => await _context.Topics.FindAsync(id);

    public async Task UpdateAsync(Topic model, IFormFile? image, string? existingPicture)
    {
        var topic = await _context.Topics.FindAsync(model.TopicId);
        if (topic == null) return;

        topic.TopicName = model.TopicName;
        topic.Description = model.Description;
        topic.VideoUrl = model.VideoUrl;
        topic.CategoryId = model.CategoryId;
        topic.PublishedAt = null;
        topic.UpdatedAt = DateTime.Now;

        if (image != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(folder);

            using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await image.CopyToAsync(stream);

            topic.TopicPicture = "/images/" + fileName;
        }
        else if (!string.IsNullOrEmpty(existingPicture))
        {
            topic.TopicPicture = existingPicture;
        }

        await _context.SaveChangesAsync();
    }

    public async Task ArchiveAsync(int id)
    {
        var topic = await _context.Topics.FirstOrDefaultAsync(t => t.TopicId == id);
        if (topic == null) return;

        _context.ArchiveTopics.Add(new ArchiveTopic
        {
            TopicName = topic.TopicName,
            Description = topic.Description,
            TopicPicture = topic.TopicPicture,
            VideoUrl = topic.VideoUrl,
            CategoryId = topic.CategoryId,
            CreatedAt = topic.CreatedAt,
            ViewCount = topic.ViewCount,
            ArchivedAt = DateTime.Now
        });

        _context.Topics.Remove(topic);
        await _context.SaveChangesAsync();
    }
}
