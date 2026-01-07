using System.ComponentModel.DataAnnotations;

namespace Neksara.Models
{
    public class TopicView
    {
        [Key]
        public int TopicViewId { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.Now;

        public int TopicId { get; set; }
        public Topic? Topic { get; set; }

        public int? IdUser { get; set; }
        public User? User { get; set; }


    }
}
