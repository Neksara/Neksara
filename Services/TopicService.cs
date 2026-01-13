using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;
using Neksara.Services.Interfaces;
using Neksara.ViewModels;
using System.Text.RegularExpressions;

namespace Neksara.Services;

public class TopicService : ITopicService
{
    private readonly ApplicationDbContext _context;

    public TopicService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔥 NEW: ambil semua topik tanpa paging
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
        model.VideoUrl = SanitizeVideoInput(model.VideoUrl);

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
        topic.VideoUrl = SanitizeVideoInput(model.VideoUrl);
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

    private string? SanitizeVideoInput(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        // Remove any script tags
        var noScripts = Regex.Replace(input, "<script[\\s\\S]*?>[\\s\\S]*?<\\/script>", string.Empty, RegexOptions.IgnoreCase);

        // Remove javascript: URIs to be safe
        noScripts = Regex.Replace(noScripts, "javascript:\\s*", string.Empty, RegexOptions.IgnoreCase);

        // Trim length to reasonable limit (e.g., 2000 chars)
        if (noScripts.Length > 2000) return noScripts.Substring(0, 2000);

        return noScripts;
    }

    public async Task ArchiveAsync(int id)
    {
        var topic = await _context.Topics
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.TopicId == id);
        if (topic == null) return;

        _context.ArchiveTopics.Add(new ArchiveTopic
        {
            OriginalTopicId = topic.TopicId,
            TopicName = topic.TopicName,
            Description = topic.Description,
            TopicPicture = topic.TopicPicture,
            VideoUrl = topic.VideoUrl,
            // avoid referencing live Category FK — store snapshot instead
            CategoryId = null,
            CategoryName = topic.Category?.CategoryName ?? string.Empty,
            CategoryPicture = topic.Category?.CategoryPicture ?? string.Empty,
            CreatedAt = topic.CreatedAt,
            UpdatedAt = topic.UpdatedAt,
            ViewCount = topic.ViewCount,
            ArchivedAt = DateTime.Now
        });

        _context.Topics.Remove(topic);
        await _context.SaveChangesAsync();
    }
}
