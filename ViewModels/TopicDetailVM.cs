namespace Neksara.ViewModels;

public class TopicDetailVM
{
    public int TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;

    public string CategoryName { get; set; }

    public string? TopicPicture { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime PublishedAt { get; set; }

}
