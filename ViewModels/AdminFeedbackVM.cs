using Neksara.Models;
using System.Collections.Generic;
public class AdminFeedbackVM
{
    public int FeedbackId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string TopicName { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int Rating { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
}
