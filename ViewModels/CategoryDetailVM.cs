using Neksara.Models;
using System.Collections.Generic;

namespace Neksara.ViewModels
{
    public class CategoryDetailVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryPicture { get; set; }
        public int TotalTopics { get; set; }
        public int TotalViews { get; set; }

        public List<TopicItemVM> Topics { get; set; } = new();
    }

    public class TopicItemVM
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
