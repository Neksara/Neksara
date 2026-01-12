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

    // ===============================
    // ADMIN INDEX (SORT + SEARCH)
    // ===============================
    public async Task<(List<CategoryIndexVM> Items, int TotalPage)>
        GetCategoryIndexAsync(string? search, string? sort, int page, int pageSize)
    {
        var query = _context.Categories
            .Include(c => c.Topics)
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.CategoryName.Contains(search));

        query = sort switch
        {
            "az" => query.OrderBy(c => c.CategoryName),
            "za" => query.OrderByDescending(c => c.CategoryName),
            "views" => query.OrderByDescending(c =>
                c.Topics.Where(t => !t.IsDeleted).Sum(t => t.ViewCount)),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        int totalData = await query.CountAsync();
        int totalPage = (int)Math.Ceiling(totalData / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync();

        return (items, totalPage);
    }

    // ===============================
    // DASHBOARD SUMMARY
    // ===============================
    public async Task<(List<CategoryIndexVM>, int)>
        GetCategorySummaryAsync(string search, string? sort, int page, int pageSize)
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
            .OrderByDescending(x => x.TotalViews)
            .Take(5)
            .ToListAsync();

        return (data, totalData);
    }

    // ===============================
    // CRUD
    // ===============================
    public async Task CreateAsync(Category category, IFormFile? image)
    {
        if (image != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(folder);

            using var stream =
                new FileStream(Path.Combine(folder, fileName), FileMode.Create);
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

    public async Task UpdateAsync(Category model, IFormFile? image)
    {
        var data = await _context.Categories.FindAsync(model.CategoryId);
        if (data == null) return;

        data.CategoryName = model.CategoryName;
        data.Description = model.Description;
        data.UpdatedAt = DateTime.Now;

        if (image != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(folder);

            using var stream =
                new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await image.CopyToAsync(stream);

            data.CategoryPicture = "/images/" + fileName;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var data = await _context.Categories.FindAsync(id);
        if (data == null) return;

        data.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    // ===============================
    // DETAIL
    // ===============================
    public async Task<CategoryDetailVM?> GetDetailAsync(int categoryId)
    {
        var category = await _context.Categories
            .Include(c => c.Topics)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && !c.IsDeleted);

        if (category == null) return null;

        return new CategoryDetailVM
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            CategoryPicture = category.CategoryPicture,
            TotalTopics = category.Topics.Count(t => !t.IsDeleted),
            TotalViews = category.Topics.Where(t => !t.IsDeleted).Sum(t => t.ViewCount),
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
                public async Task<List<Category>> GetPopularAsync(int take = 8)
            {
                return await _context.Categories
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c =>
                        c.Topics.Sum(t => t.ViewCount))
                    .Take(take)
                    .ToListAsync();
            }

}

