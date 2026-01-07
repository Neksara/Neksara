using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryPicture { get; set; } = string.Empty; 
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int ViewCount { get; set; } = 0;

        
        public ICollection<Topic> Topics { get; set; } = new HashSet<Topic>();
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        public ICollection<TopicView> TopicViews { get; set; } = new HashSet<TopicView>();
    }
}