using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;
using Neksara.Services.Interfaces;
using Neksara.ViewModels;

namespace Neksara.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔥 DASHBOARD SUMMARY
    public async Task<(List<CategoryIndexVM>, int)>
        GetCategorySummaryAsync(string search, int page, int pageSize)
    {
        var query = _context.Categories
            .Include(c => c.Topics)
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.CategoryName.Contains(search));

        int totalData = await query.CountAsync();

        var data = await query
            .Select(c => new CategoryIndexVM
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryPicture = c.CategoryPicture,
                TotalTopics = c.Topics.Count(t => !t.IsDeleted),
                TotalViews = c.Topics
                    .Where(t => !t.IsDeleted)
                    .Sum(t => t.ViewCount)
            })
            .OrderByDescending(x => x.TotalTopics)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (data, totalData);
    }

    // ===== CRUD
    public async Task<(List<Category>, int)>
        GetPagedAsync(string search, int page, int pageSize)
    {
        var query = _context.Categories.Where(c => !c.IsDeleted);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.CategoryName.Contains(search));

        int totalData = await query.CountAsync();

        var data = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (data, totalData);
    }

    // ===============================
    // CREATE WITH IMAGE
    // ===============================
    public async Task CreateAsync(Category category, IFormFile? image)
    {
        if (image != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);

            category.CategoryPicture = "/images/" + fileName;
        }

        category.CreatedAt = DateTime.Now;
        category.IsDeleted = false;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories.FindAsync(id);

    // ===============================
    // UPDATE WITH IMAGE (SAFE)
    // ===============================
    public async Task UpdateAsync(Category model, IFormFile? image)
    {
        var data = await _context.Categories.FindAsync(model.CategoryId);
        if (data == null) return;

        data.CategoryName = model.CategoryName;
        data.Description = model.Description;
        data.UpdatedAt = DateTime.Now;

        if (image != null)
        {
            // hapus image lama
            if (!string.IsNullOrEmpty(data.CategoryPicture))
            {
                var oldPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    data.CategoryPicture.TrimStart('/')
                );

                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);

            data.CategoryPicture = "/images/" + fileName;
        }

        _context.Categories.Update(data);
        await _context.SaveChangesAsync();
    }

    // ===============================
    // SOFT DELETE
    // ===============================
    public async Task SoftDeleteAsync(int id)
    {
        var data = await _context.Categories.FindAsync(id);
        if (data == null) return;

        data.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    // ===============================
    // DETAIL VIEW
    // ===============================
    public async Task<CategoryDetailVM?> GetDetailAsync(int categoryId)
    {
        var category = await _context.Categories
            .Include(c => c.Topics)
            .FirstOrDefaultAsync(c =>
                c.CategoryId == categoryId &&
                !c.IsDeleted);

        if (category == null) return null;

        return new CategoryDetailVM
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryPicture = category.CategoryPicture,

            TotalTopics = category.Topics.Count(t => !t.IsDeleted),
            TotalViews = category.Topics
                .Where(t => !t.IsDeleted)
                .Sum(t => t.ViewCount),

            Topics = category.Topics
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TopicItemVM
                {
                    TopicId = t.TopicId,
                    TopicName = t.TopicName,
                    ViewCount = t.ViewCount,
                    CreatedAt = t.CreatedAt
                })
                .ToList()
        };
    }
}
