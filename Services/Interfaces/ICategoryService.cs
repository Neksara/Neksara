using Neksara.ViewModels;
using Neksara.Models;
namespace Neksara.Services.Interfaces;
public interface ICategoryService
{
    Task<(List<Category> data, int totalData)> GetPagedAsync(string search, int page, int pageSize);
    Task<(List<CategoryIndexVM> data, int totalData)>
    GetCategorySummaryAsync(string search, int page, int pageSize);
    Task<CategoryDetailVM?> GetDetailAsync(int categoryId);
    Task CreateAsync(Category category, IFormFile? image);
    Task<Category?> GetByIdAsync(int id);
    Task UpdateAsync(Category category, IFormFile? image);  
    Task SoftDeleteAsync(int id);
    
}
