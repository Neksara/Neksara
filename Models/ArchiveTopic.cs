using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class ArchiveTopic
    {
        [Key]
        public int ArchiveTopicId { get; set; }

        // ID Topic asli (buat restore)
        public int OriginalTopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TopicPicture { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string CategoryPicture { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; }
         public DateTime UpdatedAt { get; set; }
        public DateTime ArchivedAt { get; set; } = DateTime.Now;
    }
}
