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

    // 🔥 CATEGORY DASHBOARD SUMMARY
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

    // ===== CRUD BIASA (tetap dipakai halaman lain)
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

    public async Task CreateAsync(Category category)
    {
        category.CreatedAt = DateTime.Now;
        category.IsDeleted = false;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories.FindAsync(id);

    public async Task UpdateAsync(Category category)
    {
        category.UpdatedAt = DateTime.Now;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var data = await _context.Categories.FindAsync(id);
        if (data == null) return;

        data.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
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

