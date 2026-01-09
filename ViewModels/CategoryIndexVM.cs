using Neksara.Models;
using System.Collections.Generic;

namespace Neksara.ViewModels
{
    public class CategoryIndexVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryPicture { get; set; } = string.Empty;
        public int TotalTopics { get; set; }
        public int TotalViews { get; set; }
    }
}
