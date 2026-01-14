using System.Collections.Generic;

namespace Neksara.ViewModels
{
    public class CategoryListVM
    {
        public List<CategoryCardVM> Categories { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}
