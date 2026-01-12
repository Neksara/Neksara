using Neksara.Models;

namespace Neksara.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> PopularCategories { get; set; } = new();
        public List<Topic> PopularTopics { get; set; } = new();
        public List<Testimoni> Testimonials { get; set; } = new();
    }
}
