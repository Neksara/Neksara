using Neksara.Models;
using Neksara.ViewModels;

namespace Neksara.Services.Interfaces;

public interface ICategoryService
{
    // INDEX ADMIN (SORT + SEARCH + PAGING)
    Task<(List<CategoryIndexVM> Items, int TotalPage)>
        GetCategoryIndexAsync(string? search, string? sort, int page, int pageSize);

    // DASHBOARD SUMMARY
    Task<(List<CategoryIndexVM> data, int totalData)>
        GetCategorySummaryAsync(string search, string? sort, int page, int pageSize);

    // DETAIL
    Task<CategoryDetailVM?> GetDetailAsync(int categoryId);

    // CRUD
    Task CreateAsync(Category category, IFormFile? image);
    Task<Category?> GetByIdAsync(int id);
    Task UpdateAsync(Category category, IFormFile? image);
    Task SoftDeleteAsync(int id);
    Task<List<Category>> GetPopularAsync(int take = 8);
}
