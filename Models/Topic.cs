using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class Topic
    {
        [Key]
        public int TopicId { get; set; }
        public string TopicPicture { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int ViewCount { get; set; } = 0;
        public DateTime PublishedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<TopicView> TopicViews { get; set; } = new HashSet<TopicView>();
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();

    }
}