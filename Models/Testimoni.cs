using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class Testimoni
    {
        [Key]
        public int TestimoniId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Role { get; set; }
         public int Rating { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; } = false;
        public bool IsVisible { get; set; } = true;

    }
}