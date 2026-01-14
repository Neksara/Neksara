using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        [Required]
        public string TargetType { get; set; } = null!; // Topic / Category
        [Required]
        public int TargetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public int? UserId { get; set; } = 0;
        public User? User { get; set; }

    }
}