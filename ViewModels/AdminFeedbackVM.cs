public class AdminFeedbackVM
{
    public int FeedbackId { get; set; }

    public string Name { get; set; } = "Anon";
    public string? Role { get; set; }

    public string TopicName { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public int Rating { get; set; }
    public string Description { get; set; } = string.Empty;

    public bool IsApproved { get; set; }
    public bool IsVisible { get; set; }

    // === DISPLAY ONLY ===
    public string StatusText
    {
        get
        {
            if (!IsApproved) return "Pending";
            if (!IsVisible) return "Hidden";
            return "Approved";
        }
    }
}
