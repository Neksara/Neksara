using Neksara.Models;
using System.Collections.Generic;
namespace Neksara.ViewModels;

public class TopicListVM
{
    public List<TopicCardVM> Topics { get; set; } = new();
    public int? CurrentCategory { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public DateTime? PublishedAt { get; set; } 
}
