using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Neksara.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        // =========================
        // AUTH
        // =========================
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        // =========================
        // PROFILE
        // =========================
        [MaxLength(255)]
        public string? PhotoUrl { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User";

        // =========================
        // AUDIT
        // =========================
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // =========================
        // RELATION
        // =========================
        public ICollection<TopicView> TopicViews { get; set; }
            = new HashSet<TopicView>();

        public ICollection<Feedback> Feedbacks { get; set; }
            = new HashSet<Feedback>();
    }
}
