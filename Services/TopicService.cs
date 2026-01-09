using Microsoft.EntityFrameworkCore;
using Neksara.Models;
using Neksara.Data;
using Neksara.Services.Interfaces;
using Neksara.ViewModels;

namespace Neksara.Services.Interfaces;
public class TopicService : ITopicService
{
    private readonly ApplicationDbContext _context;

    public TopicService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TopicDetailVM?> GetDetailAsync(int topicId)
    {
        var topic = await _context.Topics
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t =>
                t.TopicId == topicId &&
                !t.IsDeleted);

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
    public async Task<(List<Topic>, int)> GetPagedAsync(string search, int page, int pageSize)
    {
        var query = _context.Topics
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(t => t.TopicName.Contains(search));

        int total = await query.CountAsync();

        var data = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (data, total);
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }

    public async Task CreateAsync(Topic model, IFormFile? image)
    {
        if (image != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var path = Path.Combine("wwwroot/images", fileName);

            using var stream = new FileStream(path, FileMode.Create);
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

            public async Task UpdateAsync(
            Topic model,
            IFormFile? image,
            string? existingPicture)
        {
            var topic = await _context.Topics.FindAsync(model.TopicId);
            if (topic == null) return;

            topic.TopicName = model.TopicName;
            topic.Description = model.Description;
            topic.VideoUrl = model.VideoUrl;
            topic.CategoryId = model.CategoryId;
            topic.UpdatedAt = DateTime.Now;

            if (image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(folder);

                var path = Path.Combine(folder, fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await image.CopyToAsync(stream);

                topic.TopicPicture = "/images/" + fileName;
            }
            else if (!string.IsNullOrEmpty(existingPicture))
            {
                topic.TopicPicture = existingPicture;
            }
            // else: biarin kosong, gak masalah

            await _context.SaveChangesAsync();
        }

    public async Task ArchiveAsync(int id)
    {
        var topic = await _context.Topics
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.TopicId == id);

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
            public async Task HardDeleteArchiveAsync(int archiveTopicId)
        {
            var archive = await _context.ArchiveTopics
                .FirstOrDefaultAsync(a => a.ArchiveTopicId == archiveTopicId);

            if (archive == null) return;

            _context.ArchiveTopics.Remove(archive);
            await _context.SaveChangesAsync();
        }
}
