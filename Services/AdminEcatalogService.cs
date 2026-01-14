using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;
using Neksara.Services.Interfaces;
using Neksara.ViewModels;

namespace Neksara.Services;

public class AdminEcatalogService : IAdminEcatalogService
{
    private readonly ApplicationDbContext _context;

    public AdminEcatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Topic>, int)> GetPagedAsync(
        string? search,
        string? sort,
        int page,
        int pageSize)
    {
        var query = _context.Topics
            .Include(t => t.Category)
            .Where(t =>
                !t.IsDeleted &&
                t.PublishedAt == null   
            );

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.TopicName.Contains(search) ||
                t.Category!.CategoryName.Contains(search));
        }

        query = sort switch
        {
            "az" => query.OrderBy(t => t.TopicName),
            "za" => query.OrderByDescending(t => t.TopicName),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        int total = await query.CountAsync();

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (data, total);
    }

    // =========================
    // ARCHIVE
    // =========================
    public async Task ArchiveAsync(int[] topicIds)
    {
        var topics = await _context.Topics
            .Where(t => topicIds.Contains(t.TopicId))
            .ToListAsync();

        foreach (var t in topics)
        {
            _context.ArchiveTopics.Add(new ArchiveTopic
            {
                OriginalTopicId = t.TopicId,
                TopicName = t.TopicName,
                Description = t.Description,
                TopicPicture = t.TopicPicture,
                VideoUrl = t.VideoUrl,
                CategoryId = t.CategoryId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ViewCount = t.ViewCount
            });
        }

        _context.Topics.RemoveRange(topics);
        await _context.SaveChangesAsync();
    }

    // =========================
    // DETAIL
    // =========================
        public async Task<TopicDetailVM?> GetDetailAsync(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.TopicId == id && !t.IsDeleted);

            if (topic == null) return null;

            var picture = topic.TopicPicture;

            if (!string.IsNullOrEmpty(picture) && !picture.StartsWith("/"))
                picture = "/" + picture;

            return new TopicDetailVM
            {
                TopicId = topic.TopicId,
                TopicName = topic.TopicName,
                CategoryName = topic.Category!.CategoryName,
                ViewCount = topic.ViewCount,
                CreatedAt = topic.CreatedAt,
                UpdatedAt = topic.UpdatedAt,
                TopicPicture = picture,
                Description = topic.Description,
                VideoUrl = topic.VideoUrl
            };
        }
    // =========================
// PUBLISH
// =========================
    public async Task PublishAsync(int[] topicIds)
    {
        var topics = await _context.Topics
            .Where(t => topicIds.Contains(t.TopicId))
            .ToListAsync();

        foreach (var t in topics)
        {
            t.PublishedAt = DateTime.Now;
            t.UpdatedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }

}
