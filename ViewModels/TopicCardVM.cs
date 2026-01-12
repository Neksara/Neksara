using Neksara.Models;
using System.Collections.Generic;

namespace Neksara.ViewModels;
public class TopicCardVM
{
    public int TopicId { get; set; }
    public string TopicName { get; set; } = "";
    public string? TopicPicture { get; set; }

    public string CategoryName { get; set; } = "";
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int ViewCount { get; set; }
}
