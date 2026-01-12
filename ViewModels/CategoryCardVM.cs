using Neksara.Models;
using System.Collections.Generic;

namespace Neksara.ViewModels
{
    public class CategoryCardVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryPicture { get; set; }
        public string? Description { get; set; }
        public int TotalTopics { get; set; }
    }
}
