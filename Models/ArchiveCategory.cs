using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class ArchiveCategory
    {
        [Key]
        public int ArchiveCategoryId { get; set; }

        // ID kategori asli (untuk restore)
        public int OriginalCategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryPicture { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime ArchivedAt { get; set; } = DateTime.Now;
    }
}
